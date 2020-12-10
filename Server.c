#include <string.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <fcntl.h>
#include <stdio.h>
#include <mysql.h>
#include <pthread.h>


//Estructura usuario
typedef struct {
	char nombre[20];
	int sock;
} TUsuario;

//Lista de 100 usuarios como maximo
typedef struct{
	TUsuario usuario[100];
	int num;
} TListaUsuarios;

typedef struct {
	int ID;
	TListaUsuarios lista_jugadores;
} TPartida;

typedef struct {
	TPartida partida[100];
	int num;
} TListaPartidas;

TListaUsuarios lista_conectados;
TListaPartidas lista_partidas;

//Estructura necesaria para el acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

void AnadirConectado(char nombre[20], int socket) {
	strcpy(lista_conectados.usuario[lista_conectados.num].nombre, nombre);
	lista_conectados.usuario[lista_conectados.num].sock = socket;
	lista_conectados.num++;
}

//Retorna 1 si lo ha eliminado, -1 si el usuario no existe
int EliminarConectado(int i) {
	//Si hemos encontrado al usuario, lo eliminamos
	if(i < lista_conectados.num)
	{
		for(int j = i; j < lista_conectados.num - 1; j++)
		{
			lista_conectados.usuario[j] = lista_conectados.usuario[j+1];
		}
		lista_conectados.num--;
		
		return 1;
	}
	
	return -1;
}

void GetListaConectados(char lista[]) {
	for(int i = 0; i < lista_conectados.num; i++)
	{
		sprintf(lista, "%s%s/", lista, lista_conectados.usuario[i].nombre);
	}
	
	//eliminamos el ultimo caracter '/'
	lista[strlen(lista)-1] = '\0';
}

//Devuelve el indice donde se encuentra el usuario conectado con el socket pasado como parametro
//Retorna -1 si no existe
int GetIndex(int socket) {
	//Busqueda en la lista de conectados
	int i = 0;
	int encontrado = 0;
	
	while(i < lista_conectados.num && !encontrado)
	{
		if(lista_conectados.usuario[i].sock == socket)
			encontrado = 1;
		else
			i++;
	}
	
	if(encontrado)
		return i;
	else
		return -1;
}

void EnviarListaConectadosATodos() {
	char lista[2105];
	strcpy(lista, "");
	GetListaConectados(lista);
		
	char mensaje[2105];
	strcpy(mensaje, "");
	sprintf(mensaje, "3$%s", lista);
	
	for(int i = 0; i < lista_conectados.num; i++)
	{
		write(lista_conectados.usuario[i].sock, mensaje, strlen(mensaje));
	}
	
	printf("Enviada lista conectados a todos: %s \n", mensaje);
}

//Función que registra al usuario en la base de datos
int Registrarse (char usuario[20], char clave[20], MYSQL *conn) {	
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	
	//Antes de nada, obtenemos el numero de jugadores que hay en la base de datos para dar el identificador correcto al nuevo jugador
	char consulta [80];
	err=mysql_query (conn, "SELECT * FROM JUGADOR");
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	//Guardaremos el numero de usuarios en el parametro i
	int i = 0;
	while(row != NULL) {
		i++;
		row = mysql_fetch_row (resultado);
	}
	
	//Ahora pedimos a la base de datos que busque si hay algun nombre con el nombre que ha introducido el usuario.
	//Si es asi, no se podra registrar y tendra que introducir un nombre distinto
	strcpy(consulta, "SELECT * FROM JUGADOR WHERE JUGADOR.NOMBRE='");
	strcat(consulta, usuario);
	strcat(consulta, "'");
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	//Si la consulta tiene algun resultado, el nombre de usuario ya esta en uso.
	if(row != NULL) {
		printf("Error. Ya hay alguien con ese nombre.");
		return -2;
	}
	
	// Ahora ya podemos realizar la insercion 
	sprintf(consulta, "INSERT INTO JUGADOR VALUES (%d,'%s','%s',0, 0, 0);", i + 1, usuario, clave);
	err = mysql_query(conn, consulta);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
		mysql_errno(conn), mysql_error(conn));

		return -3;
	}
	return 0;
}

//Funcion que inicia la sesion del usuario
int IniciarSesion(char usuario[20], char clave[20], MYSQL *conn) {
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	
	//Para iniciar sesion, pedimos a la base de datos que busque jugadores que tengan el nombre y contraseña introducidos
	char consulta [80];
	strcpy(consulta, "SELECT * FROM JUGADOR WHERE JUGADOR.NOMBRE='");
	strcat(consulta, usuario);
	strcat(consulta, "' AND JUGADOR.CONTRASENA='");
	strcat(consulta, clave);
	strcat(consulta, "'");
	
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	//Si la consulta no da resultados, el usuario esta introduciendo datos erroneos
	if(row == NULL) {
		return -2;
	}
	
	return 0;
}

//Funcion que retorna los record de los jugadores que han perdido partidas contra Arnau
int PuntuacionPerdedores(char lista[100], MYSQL *conn) {
	MYSQL_RES *resultado;
	MYSQL_ROW row;	
	int err;
	
	//Realizamos la consulta necesaria en MYSQL
	char consulta [500];
	strcpy(consulta,"SELECT  DISTINCT JUGADOR.RECORD FROM (PARTIDA,JUGADOR,PARTICIPACION) WHERE PARTIDA.GANADOR = 'Arnau' AND  PARTIDA.ID = PARTICIPACION.ID_P AND PARTICIPACION.ID_J=JUGADOR.ID AND JUGADOR.NOMBRE NOT IN ('Arnau');");
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	//Si no hay resultados en la consulta significa que nunca nadie ha perdido contra Arnau
	if (row == NULL) {
		printf ("No se han obtenido datos en la consulta\n");
		strcpy(lista, "Nadie ha perdido nunca contra Arnau.");
	}
	
	//Si hay resultados, se crean a la lista que ha entrado a la funcion como parametro
	else
		while (row !=NULL) {
			sprintf(lista, "%s%s/", lista, row[0]);
			printf("%s\n", lista);
			row = mysql_fetch_row(resultado);
	}
	return 0;
}

//Funcion que retorna los nombres de los jugadores que han jugado la partida mas larga
int NombresPartidaLarga(char lista[100], MYSQL *conn) {
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	//Hacemos la consulta necesaria a la base de datos.
	char consulta[200];
	strcpy(consulta, "SELECT JUGADOR.NOMBRE FROM(PARTIDA, JUGADOR, PARTICIPACION) WHERE PARTIDA.DURACION = (SELECT MAX(DURACION) FROM PARTIDA) AND PARTIDA.ID = PARTICIPACION.ID_P AND PARTICIPACION.ID_J = JUGADOR.ID;");
	err = mysql_query(conn, consulta);
	if(err != 0){
		printf ("Error al consultar datos de la base: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	strcpy(lista, "");
	
	//Si la consulta no da resultados hay un error, ya que hay partidas en la base
	if(row == NULL) {
		printf("No se han obtenido resultados en la consulta.\n");
		return -2;
	}
	
	//Incorporamos los resultados en la lista que entra a la funcion como parametro
	else
	   while(row != NULL){
		sprintf(lista, "%s%s/", lista, row[0]);
		row = mysql_fetch_row(resultado);
	}
	   return 0;
}

//Funcion que devuelve la mayor puntuacion record registrada en la base de datos
int DameRecord(MYSQL *conn) {
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	
	//Consulta que se debe hacer en la base

	err = mysql_query(conn, "SELECT JUGADOR.FICHAS FROM JUGADOR ORDER BY JUGADOR.FICHAS DESC;");

	if(err != 0){
		printf("Error al consultar la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	//Si no hay resultados hay un error en la consulta a la base, pues en ella hay partidas guardadas
	if(row == NULL){
		printf("No se ha obtenido datos en la consulta.\n");
		return -2;
	}
	
	//Se guarda el resultado en la variable ID y la devolvemos como resultado
	int ID;
	ID = atoi(row[0]);
	return ID;
}

int AsignarIdPartida(MYSQL *conn) {
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	int id_max_sql;
	
	err = mysql_query(conn, "SELECT MAX(ID) FROM PARTIDA;");
	if(err != 0){
		printf("Error al consultar la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		return -1;
	}
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	
	//No hay partidas, asignamos id=0
	if(row == NULL){
		id_max_sql = 0;
	}
	else
	{
		//asignamos como Id el valor siguiente al valor maximo encontrado en la base de datos
		id_max_sql = atoi(row[0]) + 1;
	}
	
	//Ahora buscamos la Id maxima en la lista de partidas
	int id_max = 0;
	for(int i = 0; i < lista_partidas.num; i++)
	{
		if(id_max < lista_partidas.partida[i].ID)
			id_max = lista_partidas.partida[i].ID;
	}
	
	int id;
	if(id_max > id_max_sql)
		id = id_max + 1;
	else
		id = id_max_sql + 1;
	
	//Retornamos la Id disponible para asignar a una partida
	return id;
	
	
}

//Envia a todos los usuarios de una partida activa la lista de jugadores
//Se envia al cliente el mensaje: nombre1/nombre2/...
void EnviarListaJugadoresPartidaActiva(int i) {
	TPartida partida = lista_partidas.partida[i];
	char mensaje[180];
	
	//Creamos el mensaje
	sprintf(mensaje, "3/");
	for(int j = 0; j < partida.lista_jugadores.num; j++)
	{
		sprintf(mensaje, "%s%s/", mensaje, partida.lista_jugadores.usuario[j].nombre);
	}
	
	//Enviamos el mensaje a todos los jugadores de una partida
	for(int j = 0; j < partida.lista_jugadores.num; j++)
	{
		write (partida.lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
	}
	
}

//Funcion que realiza cada thread
void *AtenderCliente (void *socket){
	int sock_conn;
	int *k;
	k = (int *) socket;
	sock_conn = *k;

	
	MYSQL *conn;
	//Creamos una conexion al servidor MYSQL 
	conn = mysql_init(NULL);
	if (conn==NULL) {
		printf ("Error al crear la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	//inicializar la conexion, entrando nuestras claves de acceso y el nombre de la base de datos a la que queremos acceder 

	conn = mysql_real_connect (conn, "localhost","root", "mysql", "TG11",0, NULL, 0);
	//conn = mysql_real_connect (conn, "shiva2.upc.es", "root", "mysql", "TG11", 0, NULL, 0);

	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	//Variables necesarias en la counicacion con el cliente. 
	//En buff se guarda el mensaje que se lee del cliente.
	//En buff2 se guarda el mensaje que se debe enviar al cliente
	char buff[512];
	char buff2[512];
	int ret;
	
	int terminar = 0;
	while(terminar == 0) {
		// Ahora recibimos el mensaje del cliente, que dejamos en buff
		ret=read(sock_conn,buff, sizeof(buff));
		
		// Tenemos que anadirle la marca de fin de string para que no escriba lo que hay despues en el buffer
		buff[ret]='\0';
		
		//Escribimos el nombre en la consola del servidor
		printf ("Se ha recibido %s\n",buff);

		//Extraemos el codigo de peticion que nos ha enviado el cliente
		char *p = strtok(buff, "/");
		int codigo =  atoi (p);
		
		//Quiere desconectarse del servidor
		if (codigo == 0) {
				p = strtok(NULL, "/");
				int i = GetIndex(sock_conn);
				
				char nombre[20];
				if(i != -1)
				{
					strcpy(nombre, lista_conectados.usuario[i].nombre);
				}
				else
				
				//Creamos una nueva lista SIN el ususario desconectado
				pthread_mutex_lock(&mutex);
				
				EliminarConectado(i);
				
				pthread_mutex_unlock(&mutex);
				
				EnviarListaConectadosATodos();
				
				if(i != -1)
					sprintf(buff2, "Se ha desconectado %s\n", nombre);
				
				terminar=1;
			}
				
		//Quiere que lo registremos en la base de datos: 1/nombre/clave
		else if (codigo ==1){
			p = strtok(NULL, "/");
			char nombre[20];
			strcpy(nombre, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			
			//Registramos al cliente con los datos que ha enviado y analizamos resultados

			int res = Registrarse(nombre, clave, conn);

			if(res == 0)
				strcpy (buff2, "1$Se ha registrado correctamente.");
			else if(res == -1)
				strcpy (buff2, "1$Error al consultar la base de datos.");
			else if(res == -2)
				strcpy (buff2, "1$Lo sentimos, ya hay alguien con ese nombre. Introduce un nombre distinto.");
			else if(res == -3)
				strcpy (buff2, "1$Error al introducir tus datos en la base.");
			else
				printf("Error inesperado al registar al usuario.");
			
			//Enviamos el resultado
			printf("Se ha enviado %s al cliente.\n", buff2);
			write (sock_conn,buff2, strlen(buff2));
		}
		
		//Quiere iniciar sesion
		else if (codigo == 2) {
			p = strtok(NULL, "/");
			char nombre[20];
			strcpy(nombre, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			
			//Iniciamos la sesion del cliente con los datos que ha enviado y analizamos resultados

			int res = IniciarSesion(nombre, clave, conn);
			if(res ==0)
			{
				strcpy (buff2, "2$Se ha iniciado sesion correctamente.");
				AnadirConectado(nombre, sock_conn);
				
				EnviarListaConectadosATodos();
			}

			else if(res == -1)
				strcpy (buff2, "2$Error al consultar la base de datos.");
			else if(res == -2)
				strcpy (buff2, "2$Error. Los datos introducidos no coinciden.");
			else
				printf("Error inesperado.");
			
			//Enviamos el resultado
			printf("Se ha enviado %s al cliente.\n", buff2);
			write (sock_conn,buff2, strlen(buff2));
		}
		
		//Un cliente en concreto pide que se le envie la lista de conectados actualizada
		else if (codigo == 3)
		{
			//Enviamos la lista al cliente en concreto
			char lista[500];
			strcpy(lista, "");
			for(int j = 0; j < lista_conectados.num; j++)
			{
				if(strcmp(lista_conectados.usuario[j].nombre, "") != 0)
				{
					sprintf(lista, "%s%s/", lista, lista_conectados.usuario[j].nombre);
				}
			}
			//Borramos la ultima '/'
			lista[strlen(lista)-1] = '\0';
			char listadefinitiva[500];
			sprintf(listadefinitiva, "3$%s", lista);

			printf("Se ha enviado %s al cliente.\n", listadefinitiva);
			write (sock_conn, listadefinitiva, strlen(listadefinitiva));
			
			char nombre[20];
			int i = GetIndex(sock_conn);
			strcpy(nombre, lista_conectados.usuario[i].nombre);
			sprintf(buff2, "Enviada lista de conectados a %s: %d %d", nombre, sock_conn, lista_conectados.usuario[i].sock);
		}
		
		//Invitar a jugadores a la partida
		//  El servidor recibe 8/num_invitados/nombre_invitado1/nombre_invitado2/...
		else if (codigo == 8) {
			char nombre_host[20];
			int encontrado = 0;
			int l = 0;
			
			MYSQL_RES *resultado;
			MYSQL_ROW row;
			int err;
			
			//Buscamos el nombre del host en la lista de conectados
			while(l < lista_conectados.num && !encontrado) {
				if(sock_conn == lista_conectados.usuario[l].sock) {
					encontrado = 1;
					strcpy(nombre_host, lista_conectados.usuario[l].nombre);
				}
				l++;
			}
			
			pthread_mutex_lock(&mutex);
			//Creamos nueva partida en la lista de partidas
			strcpy(lista_partidas.partida[lista_partidas.num].lista_jugadores.usuario[0].nombre, nombre_host);
			lista_partidas.partida[lista_partidas.num].lista_jugadores.usuario[0].sock = sock_conn;
			
			//Asignamos una ID a la partida, miramos que no exista en la base de datos ni en la lista de partidas local
			int id = AsignarIdPartida(conn);
			lista_partidas.partida[lista_partidas.num].ID = id; //lista_partidas.num;
			
			lista_partidas.partida[lista_partidas.num].lista_jugadores.num = 1;
			
			pthread_mutex_unlock(&mutex);
			
			char mensaje[200];
			
			//Se envia al invitado: 8$nombre_host/id_partida
			sprintf(mensaje, "8$%s/%d", nombre_host, lista_partidas.partida[lista_partidas.num].ID);
			p = strtok(NULL, "/");
			int num_invitados = atoi(p);
			p = strtok(NULL, "/");
			for (int j = 0; j < num_invitados; j++) {
				char nombre_invitado[20];
				strcpy(nombre_invitado, p);
				printf("Se ha enviado %s\n", mensaje);
				for(int k = 0; k < lista_conectados.num; k++) {
					if(strcmp(lista_conectados.usuario[k].nombre, nombre_invitado) == 0)
						write (lista_conectados.usuario[k].sock, mensaje, strlen(mensaje));
				}
				p = strtok(NULL, "/");
			}
			sprintf(mensaje, "12$%d", lista_partidas.partida[lista_partidas.num].ID);
			printf("Se ha enviado %s\n", mensaje);
			write (sock_conn, mensaje, strlen(mensaje));
			lista_partidas.num++;
		}
		
		//Aceptar o rechazar la invitación
		// El servidor recibe 9/id_partida/respuesta
		//donde respuesta: 1 = SI, 0 = NO
		else if(codigo == 9) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			p = strtok(NULL, "/");
			int respuesta = atoi(p); //1 = acepta   0 = rechaza
			int l = 0;
			int encontrado = 0;
			
			//Buscamos el nombre del invitado en la lista de conectados
			char nombre_invitado[20];
			while(l < lista_conectados.num && !encontrado) {
				if(sock_conn == lista_conectados.usuario[l].sock) {
					encontrado = 1;
					strcpy(nombre_invitado, lista_conectados.usuario[l].nombre);
				} else
					l++;
			}
			
			//Buscamos la partida concreta en la lista de partidas
			// l sera el valor del indice de la partida dentro de la lista
			l = 0;
			encontrado = 0;
			while (!encontrado && l < lista_partidas.num) 
			{
					if(lista_partidas.partida[l].ID == ID_partida)
						encontrado = 1;
					else
						l++;
			}
			
			if(respuesta == 1) {
				
				//Se ha aceptado la invitacion
				//Unimos al usuario a la partida
				
				strcpy(lista_partidas.partida[l].lista_jugadores.usuario[lista_partidas.partida[l].lista_jugadores.num].nombre, nombre_invitado);
				lista_partidas.partida[l].lista_jugadores.usuario[lista_partidas.partida[l].lista_jugadores.num].sock = sock_conn;
				lista_partidas.partida[l].lista_jugadores.num++;
				
				//Enviamos a todos los jugadores de la partida la lista de jugadores actualizada
				//EnviarListaJugadoresPartidaActiva(l);
				
				//Enviamos a los jugadores de la partida un mensaje conforme el jugador ha aceptado la invitacion
				char mensaje[100];
				char mensaje2[100];
				sprintf(mensaje, "10$%d/%s se ha unido a la partida.", lista_partidas.partida[l].ID, nombre_invitado);
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[j].sock != sock_conn) {
						printf("Se ha enviado %s\n", mensaje);
						write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
					}
				}
			}
			else {
				char mensaje[100];
				sprintf(mensaje, "10$%d/%s ha rechazado unirse a la partida.", lista_partidas.partida[l].ID, nombre_invitado);
				printf("Se ha enviado %s\n", mensaje);
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
			}
		}
		
		else if(codigo == 10) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			p = strtok(NULL, "/");
			char mensaje[200];
			strcpy(mensaje, p);
			int j = 0;
			int encontrado = 0;
			while (!encontrado && j < lista_partidas.num) {
				if(lista_partidas.partida[j].ID == ID_partida) {
					encontrado = 1;
				}
				else
				   j++;
			}
			char mensaje_final[250];
			sprintf(mensaje_final, "10$%d/%s", ID_partida, mensaje);
			printf("Se ha enviado %s\n", mensaje_final);
			for(int k = 0; k < lista_partidas.partida[j].lista_jugadores.num; k++)
				write (lista_partidas.partida[j].lista_jugadores.usuario[k].sock, mensaje_final, strlen(mensaje_final));
		}
		
		else if(codigo == 11) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			int j = 0;
			int encontrado = 0;
			while (!encontrado && j < lista_partidas.num) {
				if(lista_partidas.partida[j].ID == ID_partida)
					encontrado = 1;
				else
				   j++;
			}
			int k = 0;
			encontrado = 0;
			while (!encontrado && k < lista_partidas.partida[j].lista_jugadores.num) {
				if(lista_partidas.partida[j].lista_jugadores.usuario[k].sock == sock_conn)
					encontrado = 1;
				else
				   k++;
			}
			strcpy(lista_partidas.partida[j].lista_jugadores.usuario[k].nombre, "");
			lista_partidas.partida[j].lista_jugadores.usuario[k].sock = -1;
			lista_partidas.partida[j].lista_jugadores.num--;
		}
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);
}

//Inicio del código
int main(int argc, char *argv[]){
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	lista_conectados.num = 0;
	lista_partidas.num = 0;
	
	//Inicializaciones
	//Primero, abrimos el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creando socket");
	
	//A continuación, hacemos el bind al puerto
	memset(&serv_adr, 0, sizeof(serv_adr)); // inicializa a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	//Asocia el socket a cualquiera de las IP de la maquina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	//Escucharemos en el puerto indicado entre parenteis
	serv_adr.sin_port = htons(9050);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen\n");
	
	pthread_t thread;
	
	
	// Atendemos peticiones
	for(;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		
		
		//Crear thead y decirle lo que tiene que hacer
		pthread_create (&thread, NULL, AtenderCliente, &sock_conn);

	}
}

