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

//Lista de nombres de los usuarios conectados

//Estructura necesaria para el acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

//Funci??n que registra al usuario en la base de datos
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
	sprintf(consulta, "INSERT INTO JUGADOR VALUES (%d,'%s','%s',0,0,0);", i + 1, usuario, clave);
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
	
	//Para iniciar sesion, pedimos a la base de datos que busque jugadores que tengan el nombre y contrase??a introducidos
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
		printf("Error. Los datos no coinciden.");
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

typedef struct {
	char nombre [20];
	int socket;
}Conectado;

typedef struct{
	Conectado conectados [100];
	int num;
}ListaConectados;

int Pon (ListaConectados *lista, char nombre[20],int socket){
	//A?ade nuevo Conectado
	//retorna 0 si ok y -1 si la lista ya estaba llena
	if (lista->num == 100)
		return -1;
	else {
		strcpy(lista->conectados[lista->num].nombre ,nombre);
		lista->conectados[lista->num].socket = socket;
		lista->num++;
		return 0;
	}
}

int DamePosicion (ListaConectados *lista, char nombre[20]){
	//Devuelve la posicion lista o -1 si no esta en la lista
	int i = 0;
	int encontrado = 0;
	while ((i<lista->num) && !encontrado)
	{
		if (strcmp(lista->conectados[i].nombre ,nombre)==0)
			encontrado = 1;
		if (!encontrado)
			i++;
		
	}
	if (encontrado)
		return i;
	else 
		return -1;
	
}

int Elimina (ListaConectados *lista, char nombre[20])
{
	//Retorna 0 si elimina y -1 si ese usuario no esta en la lista
	int pos = DamePosicion (lista,nombre);
	if (pos==-1)
		return -1;
	else 
	{
		int i;
		for (i=pos; i< lista->num-1; i++)
		{
			lista->conectados[i] = lista->conectados[i+1];
			//strcpy(lista->conectados[i].nombre , lista->conectados[i+1].nombre);
			//lista->conectados[i].socket = lista->conectados[i+1].socket;
		}
		lista->num--;
		return 0;
		
	}
}

void DameConectados (ListaConectados *lista, char conectados[520]){
	//Pone en conectados los nombres de todos los conectados separados por barra
	//Primero pone el n?mero de conectados. P.ej: "2/Juan/Pablo"
	sprintf(conectados, "%d",lista->num);
	int i;
	for (i=0; i< lista->num; i++)
		sprintf(conectados,"%s/%s",conectados,lista->conectados[i].nombre);
	
}


ListaConectados miLista;
//Funcion que realiza cada thread
void *AtenderCliente (void *socket){
	int sock_conn;
	int *s;
	s= (int *) socket;
	sock_conn= *s;
	
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
		printf ("Recibido\n");
		
		// Tenemos que anadirle la marca de fin de string para que no escriba lo que hay despues en el buffer
		buff[ret]='\0';
		char nombre_usuario [20];
		//Escribimos el nombre en la consola del servidor
		printf ("Se ha conectado: %s\n",buff);
		
		//Extraemos el codigo de peticion que nos ha enviado el cliente
		char *p = strtok(buff, "/");
		int codigo =  atoi (p);
		
		//Quiere que lo registremos en la base de datos
		if (codigo ==1){
			p = strtok(NULL, "/");
			char usuario[20];
			strcpy(nombre_usuario, usuario);
			strcpy(usuario, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			
			//Registramos al cliente con los datos que ha enviado y analizamos resultados
			int res = Registrarse(usuario, clave, conn);
			if(res == 0)
				strcpy (buff2, "Se ha registrado correctamente.");
			else if(res == -1)
				strcpy (buff2, "Error al consultar la base de datos.");
			else if(res == -2)
				strcpy (buff2, "Lo sentimos, ya hay alguien con ese nombre. Introduce un nombre distinto.");
			else if(res == -3)
				strcpy (buff2, "Error al introducir tus datos en la base.");
			else
				printf("Error inesperado al registar al usuario.");
		}
		
		//Quiere iniciar sesion
		else if (codigo == 2) {
			p = strtok(NULL, "/");
			char usuario[20];
			strcpy(usuario, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			
			pthread_mutex_lock(&mutex);
			Pon (&miLista,usuario,sock_conn);
			pthread_mutex_unlock(&mutex);
			
			
			//Iniciamos la sesion del cliente con los datos que ha enviado y analizamos resultados
			int res = IniciarSesion(usuario, clave, conn);
			if(res ==0)
				strcpy (buff2, "Se ha iniciado sesion correctamente.");
			else if(res == -1)
				strcpy (buff2, "Error al consultar la base de datos.");
			else if(res == -2)
				strcpy (buff2, "Error. Los datos introducidos no coinciden.");
			else
				printf("Error inesperado.");
		}
		
		//Quiere obtener los record de los jugadores que han perdido partidas contra Arnau
		else if (codigo == 3) {
			char lista[100];
			strcpy(lista, "");
			
			//Llamamos a la funcion para que haga la consulta necesaria y recogemos el resultado guardado en lista. Analizamos resultados
			int res = PuntuacionPerdedores(lista, conn);
			printf("%s\n", lista);
			if(res == 0)
				strcpy (buff2, lista);
			else if(res == -1)
				strcpy (buff2, " (Error al consultar la base de datos)");
			else
				printf("Error inesperado.");
		}
		
		//Quiere obtener los nombres de los jugadores que han jugado la partida mas larga
		else if (codigo == 4) {
			char lista[100];
			
			//Llamamos a la funcion para que haga la consulta necesaria y recogemos el resultado guardado en lista. Analizamos resultados
			int res = NombresPartidaLarga(lista, conn);
			if(res == 0)
				strcpy (buff2, lista);
			else if(res == -1)
				strcpy (buff2, " (Error al consultar la base de datos)");
			else if(res == -2)
				strcpy (buff2, " (Error. La consulta no ha ofrecido resultados)");
			else
				printf("Error inesperado.");
		}
		
		//Quiere obtener el identificador de la persona que ostenta el mayor record de la base de datos
		else if (codigo == 5) {
			//Llamamos a la funcion para que haga la consulta necesaria y analizamos resultados
			int res = DameRecord(conn);
			if(res > 0)
				sprintf (buff2, "%d", res);
			else if(res == -1)
				strcpy (buff2, " (Error al consultar la base de datos)");
			else if(res == -2)
				strcpy (buff2, " (Error. La consulta no ha ofrecido resultados)");
		}
		
		//Quiere obtener la lista de usuarios conectados al servidor
		else if (codigo == 6){
			
			pthread_mutex_lock(&mutex);
			DameConectados(&miLista, buff2);			
			pthread_mutex_unlock(&mutex);
			
		}
		
		//Quiere desconectarse del servidor
		else if (codigo == 0) {
			
			pthread_mutex_lock(&mutex);
			Elimina(&miLista,nombre_usuario);
			pthread_mutex_unlock(&mutex);
			
			terminar=1;
		}
		printf ("%s\n", buff2);
		//Enviamos el resultado
		write (sock_conn,buff2, strlen(buff2));
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);
}


//Inicio del c??digo
int main(int argc, char *argv[]){
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	miLista.num = 0;
	//Inicializaciones
	//Primero, abrimos el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	
	//A continuaci??n, hacemos el bind al puerto
	memset(&serv_adr, 0, sizeof(serv_adr)); // inicializa a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	//Asocia el socket a cualquiera de las IP de la maquina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	
	//Escucharemos en el puerto indicado entre parenteis
	serv_adr.sin_port = htons(9080);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen");
	
	int i;
	int sockets[i];
	pthread_t thread;
	i=0;
	
	// Atendemos peticiones
	for(;;){
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		
		//sock_conn es el socket que usaremos para este cliente
		sockets[i] = sock_conn;
		
		//Crear thead y decirle lo que tiene que hacer
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
		i=i+1;
	}
}

