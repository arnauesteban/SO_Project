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
#include <time.h>


//Estructura usuario
typedef struct {
	char nombre[20];
	int sock;
} TUsuario;

//Lista de 100 usuarios como maximo
typedef struct {
	TUsuario usuario[100];
	int num;
} TListaUsuarios;

typedef struct {
	char nombre[20];
	int sock;
	int fichas;
	int jugado;
	char estado; //F = fold, C = call, R = raise, W = waiting, A = all in, O = out of money
} TJugador;

typedef struct {
	TJugador usuario[100];
	int num;
} TListaJugadores;

typedef struct {
	int ID;
	TListaJugadores lista_jugadores;
	int baraja[52];
	int ciega;
	int dineroInicial;
	char host[20];
	char dealer[20];
	int puja;
	int suma;
	char estado; //B = beginning, F = flop, T = turn, R = river, E = ending
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

int EliminarConectado(int i) {
	//Retorna 1 si lo ha eliminado, -1 si el usuario no existe
	
	//Si hemos encontrado al usuario, lo eliminamos
	if(i < lista_conectados.num) {
		for(int j = i; j < lista_conectados.num - 1; j++)
			lista_conectados.usuario[j] = lista_conectados.usuario[j+1];
		lista_conectados.num--;
		return 1;
	}
	return -1;
}

void GetListaConectados(char lista[]) {
	for(int i = 0; i < lista_conectados.num; i++)
		sprintf(lista, "%s%s/", lista, lista_conectados.usuario[i].nombre);
	
	//eliminamos el ultimo caracter '/'
	lista[strlen(lista)-1] = '\0';
}

int GetIndex(int socket) {
	//Devuelve el indice donde se encuentra el usuario conectado con el socket pasado como parametro
	//Retorna -1 si no existe
	//Busqueda en la lista de conectados
	int i = 0;
	int encontrado = 0;
	
	while(i < lista_conectados.num && !encontrado) {
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
		write(lista_conectados.usuario[i].sock, mensaje, strlen(mensaje));
	
	printf("Enviada lista conectados a todos: %s \n", mensaje);
}

int Registrarse (char usuario[20], char clave[20], MYSQL *conn) {	
	//Función que registra al usuario en la base de datos
	
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
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
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

int IniciarSesion(char usuario[20], char clave[20], MYSQL *conn) {
	//Funcion que inicia la sesion del usuario
	
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
	if(row == NULL)
		return -2;
	
	return 0;
}

int PuntuacionPerdedores(char lista[100], MYSQL *conn) {
	//Funcion que retorna los record de los jugadores que han perdido partidas contra Arnau
	
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

int NombresPartidaLarga(char lista[100], MYSQL *conn) {
	//Funcion que retorna los nombres de los jugadores que han jugado la partida mas larga
	
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

int DameRecord(MYSQL *conn) {
	//Funcion que devuelve la mayor puntuacion record registrada en la base de datos
	
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
	if(row == NULL)
		id_max_sql = 0;
	else
	{
		//asignamos como Id el valor siguiente al valor maximo encontrado en la base de datos
		id_max_sql = atoi(row[0]) + 1;
	}
	
	//Ahora buscamos la Id maxima en la lista de partidas
	int id_max = 0;
	for(int i = 0; i < lista_partidas.num; i++)
		if(id_max < lista_partidas.partida[i].ID)
			id_max = lista_partidas.partida[i].ID;
	
	int id;
	if(id_max > id_max_sql)
		id = id_max + 1;
	else
		id = id_max_sql + 1;
	
	//Retornamos la Id disponible para asignar a una partida
	return id;	
}

void EnviarMensaje(char mensaje[100], int ID) {
	int j = 0;
	int encontrado = 0;
	while (!encontrado && j < lista_partidas.num) {
		if(lista_partidas.partida[j].ID == ID)
			encontrado = 1;
		else
		    j++;
	}
	char mensaje_final[250];
	sprintf(mensaje_final, "6$%d/%s", ID, mensaje);
	printf("Se ha enviado %s a la partida %d\n", mensaje_final, ID);
	for(int k = 0; k < lista_partidas.partida[j].lista_jugadores.num; k++)
		write (lista_partidas.partida[j].lista_jugadores.usuario[k].sock, mensaje_final, strlen(mensaje_final));
}

void BarajarCartas(int l) {
	for(int k = 0; k < 52; k++)
		lista_partidas.partida[l].baraja[k] = -1;
	int i = 0; 
	while(i < 52) {
		int test = rand() % 52;
		int j = 0;
		int encontrado = 0;
		while (j < 52 && !encontrado) {
			if(lista_partidas.partida[l].baraja[i] == test)
				encontrado = 1;
			else
				j++;
		}
		if(!encontrado) {
			lista_partidas.partida[l].baraja[i] = test;
			i++;
		}
	}
}

void *AtenderCliente (void *socket){
	//Funcion que ejecuta cada thread
	
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
		printf ("Error al inicializar la conexion: %u %s\n", mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	
	srand(time(NULL));
	
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
		//El servidor recibe 0/
		if (codigo == 0) {
				p = strtok(NULL, "/");
				int i = GetIndex(sock_conn);
				
				char nombre[20];
				if(i != -1)
					strcpy(nombre, lista_conectados.usuario[i].nombre);
				
				//Creamos una nueva lista SIN el ususario desconectado
				pthread_mutex_lock(&mutex);
				
				EliminarConectado(i);
				
				pthread_mutex_unlock(&mutex);
				
				EnviarListaConectadosATodos();
				
				if(i != -1)
					sprintf(buff2, "Se ha desconectado %s\n", nombre);
				
				terminar=1;
			}
				
		//Quiere que lo registremos en la base de datos
		//El servidor recibe 1/nombre/clave
		else if (codigo == 1) {
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
		//El servidor recibe 2/nombre/clave
		else if (codigo == 2) {
			p = strtok(NULL, "/");
			char nombre[20];
			strcpy(nombre, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			
			//Iniciamos la sesion del cliente con los datos que ha enviado y analizamos resultados

			int res = IniciarSesion(nombre, clave, conn);
			if(res ==0) {
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
		
		//Un cliente en concreto quiere que se le envie la lista de conectados actualizada
		//El servidor recibe 3/
		else if (codigo == 3) {
			//Enviamos la lista al cliente en concreto
			char lista[500];
			strcpy(lista, "");
			for(int j = 0; j < lista_conectados.num; j++)
				if(strcmp(lista_conectados.usuario[j].nombre, "") != 0)
					sprintf(lista, "%s%s/", lista, lista_conectados.usuario[j].nombre);
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
		
		//Quiere invitar a jugadores a una partida
		//  El servidor recibe 4/num_invitados/nombre_invitado1/nombre_invitado2/...
		else if (codigo == 4) {
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
			lista_partidas.partida[lista_partidas.num].ID = id;
			
			//Ponemos al jugador que ha creado la partida como host de la partida.
			strcpy(lista_partidas.partida[lista_partidas.num].host, nombre_host);
			
			lista_partidas.partida[lista_partidas.num].lista_jugadores.num = 1;
			
			pthread_mutex_unlock(&mutex);
			
			char mensaje[200];
			
			//Se envia al invitado: 5$nombre_host/id_partida
			sprintf(mensaje, "5$%s/%d", nombre_host, lista_partidas.partida[lista_partidas.num].ID);
			p = strtok(NULL, "/");
			int num_invitados = atoi(p);
			p = strtok(NULL, "/");
			for (int j = 0; j < num_invitados; j++) {
				char nombre_invitado[20];
				strcpy(nombre_invitado, p);
				printf("Se ha enviado %s\n", mensaje);
				for(int k = 0; k < lista_conectados.num; k++)
					if(strcmp(lista_conectados.usuario[k].nombre, nombre_invitado) == 0)
						write (lista_conectados.usuario[k].sock, mensaje, strlen(mensaje));
				p = strtok(NULL, "/");
			}
			sprintf(mensaje, "4$%d", lista_partidas.partida[lista_partidas.num].ID);
			printf("Se ha enviado %s\n", mensaje);
			write (sock_conn, mensaje, strlen(mensaje));
			lista_partidas.num++;
		}
		
		//Quiere aceptar o rechazar una invitación recibida
		// El servidor recibe 5/id_partida/respuesta (donde respuesta: 1 = SI, 0 = NO)
		else if (codigo == 5) {
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
			while (!encontrado && l < lista_partidas.num) {
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
				char mensaje2[200];
				strcpy(mensaje2, "");
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					sprintf(mensaje2, "%s%s/", mensaje2, lista_partidas.partida[l].lista_jugadores.usuario[j].nombre);
				//eliminamos el ultimo caracter '/'
				mensaje2[strlen(mensaje2)-1] = '\0';
				char mensaje2final[200];
				sprintf(mensaje2final, "8$%d/2/%d/%d/%d/%s", ID_partida, lista_partidas.partida[l].dineroInicial, lista_partidas.partida[l].ciega, lista_partidas.partida[l].lista_jugadores.num, mensaje2);
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					write(lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje2final, strlen(mensaje2final));
				printf("Se ha enviado %s\n", mensaje2final);
				
				//Enviamos a los jugadores de la partida un mensaje conforme el jugador ha aceptado la invitacion
				char mensaje[100];
				sprintf(mensaje, "6$%d/%s se ha unido a la partida.", lista_partidas.partida[l].ID, nombre_invitado);
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					if(lista_partidas.partida[l].lista_jugadores.usuario[j].sock != sock_conn) {
						printf("Se ha enviado %s\n", mensaje);
						write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
					}
			}
			else {
				char mensaje[100];
				sprintf(mensaje, "6$%d/%s ha rechazado unirse a la partida.", lista_partidas.partida[l].ID, nombre_invitado);
				printf("Se ha enviado %s\n", mensaje);
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
			}
		}
		
		//Quiere enviar mensajes al chat de una partida
		//El servidor recibe 6/id_partida/mensaje
		else if (codigo == 6) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			p = strtok(NULL, "/");
			char mensaje[200];
			strcpy(mensaje, p);
			EnviarMensaje(mensaje, ID_partida);
		}
		
		//Quiere salir de una partida
		//El servidor recibe 7/id_partida
		else if (codigo == 7) {
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
			encontrado = 0;
			int l = 0;
			int index;
			if(strcmp(lista_partidas.partida[j].lista_jugadores.usuario[k].nombre, lista_partidas.partida[j].host) == 0) {
				if (k == lista_partidas.partida[j].lista_jugadores.num)
					index = 0;
				else
					index = k + 1;
				while(!encontrado && l < index != k) {
					if(strcmp(lista_partidas.partida[j].lista_jugadores.usuario[index].nombre, "") != 0)
						encontrado = 1;
					else {
						if (index == lista_partidas.partida[j].lista_jugadores.num)
							index = 0;
						else
							index++;
					}
				}
				if(encontrado) {
					char mensaje[100];
					sprintf(mensaje, "7$%d", ID_partida);
					write(lista_partidas.partida[j].lista_jugadores.usuario[index].sock, mensaje, strlen(mensaje));
					char mensaje2[100];
					sprintf(mensaje2, "%s es el nuevo host.", lista_partidas.partida[j].lista_jugadores.usuario[index].nombre);
					EnviarMensaje(mensaje2, ID_partida);
				}
			}
			strcpy(lista_partidas.partida[j].lista_jugadores.usuario[k].nombre, "");
			lista_partidas.partida[j].lista_jugadores.usuario[k].sock = -1;
			lista_partidas.partida[j].lista_jugadores.num--;
		}
	
		//Quiere configurar los parametros iniciales de una partida
		//El servidor recibe 8/id_partida/dineroInicial/ciega
		else if (codigo == 8) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			p = strtok(NULL, "/");
			int numConfig = atoi(p);
			
			//Buscamos la partida concreta en la lista de partidas
			// l sera el valor del indice de la partida dentro de la lista
			int l = 0;
			int encontrado = 0;
			while (!encontrado && l < lista_partidas.num) {
				if(lista_partidas.partida[l].ID == ID_partida)
					encontrado = 1;
				else
					l++;
			}
			
			//Enviar cambios al resto de jugadores
			if(numConfig == 0)
			{
				p = strtok(NULL, "/");
				int dinero = atoi(p);
				lista_partidas.partida[ID_partida].dineroInicial = dinero;
				p = strtok(NULL, "/");
				int ciega = atoi(p);
				lista_partidas.partida[ID_partida].ciega = ciega;
				char mensaje[100];
				sprintf(mensaje, "8$%d/0/%d/%d", ID_partida, dinero, ciega);
				printf("Se ha enviado %s.\n", mensaje);
				
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					if(lista_partidas.partida[l].lista_jugadores.usuario[j].sock != sock_conn)
						write(lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
			}
			else if(numConfig == 2)
			{
				char mensaje2[200];
				strcpy(mensaje2, "");
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					sprintf(mensaje2, "%s%s/", mensaje2, lista_partidas.partida[l].lista_jugadores.usuario[j].nombre);
				//eliminamos el ultimo caracter '/'
				mensaje2[strlen(mensaje2)-1] = '\0';
				char mensaje2final[200];
				sprintf(mensaje2final, "8$%d/2/%d/%d/%d/%s", ID_partida, lista_partidas.partida[l].dineroInicial, lista_partidas.partida[l].ciega, lista_partidas.partida[l].lista_jugadores.num, mensaje2);
				printf("Se ha enviado %s.\n", mensaje2final);
				write(sock_conn, mensaje2final, strlen(mensaje2final));
			}
		}
	
		//Quiere comenzar una partida
		//El servidor recibe 9/id_partida
		else if (codigo == 9) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			char mensaje[100];
			strcpy(mensaje, "Comienza la partida");
			EnviarMensaje(mensaje, ID_partida);
			
			//Buscamos la partida en la lista
			int l = 0;
			int encontrado = 0;
			while (!encontrado && l < lista_partidas.num) {
				if(lista_partidas.partida[l].ID == ID_partida)
					encontrado = 1;
				else
					l++;
			}
			
			pthread_mutex_lock(&mutex);
			
			lista_partidas.partida[l].puja = 2 * lista_partidas.partida[l].ciega;
			lista_partidas.partida[l].suma = 3 * lista_partidas.partida[l].ciega;
			
			lista_partidas.partida[l].estado = 'B';
			
			//Asignamos al host de la partida como dealer de la primera ronda
			strcpy(lista_partidas.partida[l].dealer, lista_partidas.partida[l].host);
			
			//Configuramos la cantidad inicial de fichas de cada jugador
			for(int k = 0; k < lista_partidas.partida[l].lista_jugadores.num; k++) {
				lista_partidas.partida[l].lista_jugadores.usuario[k].fichas = lista_partidas.partida[l].dineroInicial;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'W';
			}
			
			//Contabilizamos las apuestas ciegas de los jugadores que las deben hacer
			lista_partidas.partida[l].lista_jugadores.usuario[1].jugado = lista_partidas.partida[l].ciega;
			if(lista_partidas.partida[l].lista_jugadores.num > 2)
				lista_partidas.partida[l].lista_jugadores.usuario[2].jugado = 2 * lista_partidas.partida[l].ciega;
			else
				lista_partidas.partida[l].lista_jugadores.usuario[0].jugado = 2 * lista_partidas.partida[l].ciega;
			
			//Barajamos las cartas
			BarajarCartas(l);
			pthread_mutex_unlock(&mutex);
			
			//Repartimos las cartas
			int carta = 0;
			int j = 0;
			while(j < lista_partidas.partida[l].lista_jugadores.num) {
				char mensaje[100];
				sprintf(mensaje, "9$%d/0/%d/%d/%s", ID_partida, lista_partidas.partida[l].baraja[carta], 
						lista_partidas.partida[l].baraja[carta + 1], lista_partidas.partida[l].dealer);
				write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
				printf("Se ha enviado %s\n", mensaje);
				
				carta = carta + 2;
				j++;
			}
			char mensaje2[100];
			if(lista_partidas.partida[l].lista_jugadores.num > 3)
				sprintf(mensaje2, "11$%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[3].nombre);
			else
				sprintf(mensaje2, "11$%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[3 - lista_partidas.partida[l].lista_jugadores.num].nombre);
			for(j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
				write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje2, strlen(mensaje2));
			printf("Se ha enviado %s\n", mensaje2);
		}
	
		//Ha hecho una accion en una partida (ir, no ir o subir)
		//El servidor recibe 10/id_partida/accion (-1 si no va, N si apuesta N fichas)
		else if (codigo == 10) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			p = strtok(NULL, "/");
			int accion = atoi(p);
			
			//Buscamos la partida en la lista
			int l = 0;
			int encontrado = 0;
			while (!encontrado && l < lista_partidas.num) {
				if(lista_partidas.partida[l].ID == ID_partida)
					encontrado = 1;
				else
					l++;
			}
			
			//Buscamos al jugador en la lista de la partida
			int k = 0;
			encontrado = 0;
			while (!encontrado && k < lista_partidas.partida[l].lista_jugadores.num) {
				if(lista_partidas.partida[l].lista_jugadores.usuario[k].sock == sock_conn)
					encontrado = 1;
				else
					k++;
			}
			
			//Enviamos la acción del jugador al resto de jugadores
			char mensaje0[100];
			sprintf(mensaje0, "10$%d/%d/%d", ID_partida, k, accion);
			EnviarMensaje(mensaje0, ID_partida);
			//Miramos que accion ha realizado el cliente
			pthread_mutex_lock(&mutex);
			//Ha dejado sus cartas en la mesa
			if(accion == -1) {
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'F';
				char mensaje[100];
				sprintf(mensaje, "%s no va.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre);
				EnviarMensaje(mensaje, ID_partida);
			}
			
			//Se juega todas las fichas que tiene
			else if(accion + lista_partidas.partida[l].lista_jugadores.usuario[k].jugado >= lista_partidas.partida[l].lista_jugadores.usuario[k].fichas) {
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado = lista_partidas.partida[l].lista_jugadores.usuario[k].fichas;
				if(lista_partidas.partida[l].puja < lista_partidas.partida[l].lista_jugadores.usuario[k].fichas)
					lista_partidas.partida[l].puja = lista_partidas.partida[l].lista_jugadores.usuario[k].fichas;
				lista_partidas.partida[l].suma += accion;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'A';
				char mensaje[100];
				sprintf(mensaje, "%s lo apuesta todo: %d fichas.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre, lista_partidas.partida[l].lista_jugadores.usuario[k].fichas);
				EnviarMensaje(mensaje, ID_partida);
			}
			
			//Ve la apuesta
			else if(accion + lista_partidas.partida[l].lista_jugadores.usuario[k].jugado == lista_partidas.partida[l].puja) {
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado += accion;
				lista_partidas.partida[l].suma += accion;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'C';
				char mensaje[100];
				sprintf(mensaje, "%s ve la apuesta: %d fichas.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre, accion);
				EnviarMensaje(mensaje, ID_partida);
			}
			
			//Sube la apuesta
			else if(accion + lista_partidas.partida[l].lista_jugadores.usuario[k].jugado > lista_partidas.partida[l].puja) {
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado += accion;
				lista_partidas.partida[l].puja = lista_partidas.partida[l].lista_jugadores.usuario[k].jugado;
				lista_partidas.partida[l].suma += accion;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'R';
				char mensaje[100];
				sprintf(mensaje, "%s sube la apuesta: %d fichas.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre, accion);
				EnviarMensaje(mensaje, ID_partida);
			}
			else
				printf("Error inesperado en la acción del jugador.");
			pthread_mutex_unlock(&mutex);
			
			//Verificamos que la fase ha terminado
			int retirados = 0;
			int resto = 0;
			
			//Comprobamos el estado de los jugadores
			for(int n = 0; n < lista_partidas.partida[l].lista_jugadores.num; n++) {
				if(lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'F' || lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'O')
					retirados++;
				else if(lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'C'|| lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'R' || lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'A')
					resto++;
			}
			
			//Solo queda un jugador en la ronda. Gana la apuesta y se pasa a la ronda siguiente
			if(retirados + resto == lista_partidas.partida[l].lista_jugadores.num && resto == 1) {
				pthread_mutex_lock(&mutex);
				for(int n = 0; n < lista_partidas.partida[l].lista_jugadores.num; n++) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'C'|| lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'R' || lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'A') {
						lista_partidas.partida[l].lista_jugadores.usuario[n].fichas += (lista_partidas.partida[l].suma - lista_partidas.partida[l].lista_jugadores.usuario[n].jugado);
						char mensaje[100];
						sprintf(mensaje, "%s ha ganado la ronda. Se lleva %d fichas.", lista_partidas.partida[l].lista_jugadores.usuario[n].nombre, lista_partidas.partida[l].suma);
						EnviarMensaje(mensaje, ID_partida);
					}
					else
						lista_partidas.partida[l].lista_jugadores.usuario[n].fichas -= lista_partidas.partida[l].lista_jugadores.usuario[n].jugado;
					if(lista_partidas.partida[l].lista_jugadores.usuario[n].fichas == 0)
						lista_partidas.partida[l].lista_jugadores.usuario[n].estado = 'O';
					else
						lista_partidas.partida[l].lista_jugadores.usuario[n].estado = 'W';
					lista_partidas.partida[l].lista_jugadores.usuario[n].jugado = 0;
				}
				lista_partidas.partida[l].suma = 0;
				lista_partidas.partida[l].puja = 0;
				pthread_mutex_unlock(&mutex);
				
				//Pasamos el dealer al siguiente jugador
				int m = 0;
				encontrado = 0;
				while(m < lista_partidas.partida[l].lista_jugadores.num && !encontrado) {
					if(strcmp(lista_partidas.partida[l].lista_jugadores.usuario[m].nombre, lista_partidas.partida[l].dealer) == 0)
						encontrado = 1;
					else
						m++;
				}
				int o;
				if (m == lista_partidas.partida[l].lista_jugadores.num)
					o = 0;
				else
					o = m + 1;
				encontrado = 0;
				while(m != o && !encontrado) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[o].estado == 'W')
						encontrado = 1;
					else if (m == lista_partidas.partida[l].lista_jugadores.num)
						o = 0;
					else
						o++;
				}
				pthread_mutex_lock(&mutex);
				
				lista_partidas.partida[l].puja = 2 * lista_partidas.partida[l].ciega;
				lista_partidas.partida[l].suma = 3 * lista_partidas.partida[l].ciega;
				
				lista_partidas.partida[l].estado = 'B';
				
				strcpy(lista_partidas.partida[l].dealer, lista_partidas.partida[l].lista_jugadores.usuario[o].nombre);
				
				//Contabilizamos las apuestas ciegas de los jugadores que las deben hacer
				if (o == lista_partidas.partida[l].lista_jugadores.num)
					m = 0;
				else
					m = o + 1;
				encontrado = 0;
				while(!encontrado && m != o) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'O')
						encontrado = 1;
					else if(m == lista_partidas.partida[l].lista_jugadores.num)
						m = 0;
					else
						m++;
				}
				lista_partidas.partida[l].lista_jugadores.usuario[m].jugado = lista_partidas.partida[l].ciega;
				if(m == lista_partidas.partida[l].lista_jugadores.num)
					m = 0;
				else
					m++;
				encontrado = 0;
				while(!encontrado && m != o) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'O')
						encontrado = 1;
					else if(m == lista_partidas.partida[l].lista_jugadores.num)
						m = 0;
					else
						m++;
				}
				lista_partidas.partida[l].lista_jugadores.usuario[m].jugado = 2 * lista_partidas.partida[l].ciega;
				
				//Barajamos las cartas
				BarajarCartas(l);
				pthread_mutex_unlock(&mutex);
				
				//Repartimos las cartas
				int carta = 0;
				int j = 0;
				while(j < lista_partidas.partida[l].lista_jugadores.num) {
					char mensaje[100];
					sprintf(mensaje, "9$%d/0/%d/%d/%s", ID_partida, lista_partidas.partida[l].baraja[carta], 
							lista_partidas.partida[l].baraja[carta + 1], lista_partidas.partida[l].dealer);
					write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
					
					carta = carta + 2;
					j++;
				}
				if(m == lista_partidas.partida[l].lista_jugadores.num)
					  m = 0;
				else
					m++;
				encontrado = 0;
				while(!encontrado && m != o) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'O')
						encontrado = 1;
					else if(m == lista_partidas.partida[l].lista_jugadores.num)
						m = 0;
					else
						m++;
				}
				char mensaje2[100];
				sprintf(mensaje2, "11$%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[m].nombre);
				write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje2, strlen(mensaje2));
			}
			
			//Comprobamos que todos los jugadores que continuan en la ronda tengan las mismas fichas en juego (excepto all in)
			//Si esto se cumple se pasa de ronda. Si no, se pasa el turno al siguiente jugador activo en la ronda
			else if(retirados + resto == lista_partidas.partida[l].lista_jugadores.num) {
				int n = 0;
				int encontrado = 0;
				while(!encontrado && n < lista_partidas.partida[l].lista_jugadores.num) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[k].estado == 'C' || lista_partidas.partida[l].lista_jugadores.usuario[n].estado == 'R') {
						if(lista_partidas.partida[l].lista_jugadores.usuario[k].fichas != lista_partidas.partida[l].puja)
							encontrado = 1;
						else
							n++;
					}
					else
					    n++;
				}
				
				if(!encontrado) {
					pthread_mutex_lock(&mutex);
					//Pasamos de fase
					if(lista_partidas.partida[l].estado == 'B') {
						lista_partidas.partida[l].estado = 'F';
						char mensaje[100];
						sprintf(mensaje, "9$%d/1/%d/%d/%d", ID_partida, lista_partidas.partida[l].baraja[2 * lista_partidas.partida[l].lista_jugadores.num], 
								lista_partidas.partida[l].baraja[2 * lista_partidas.partida[l].lista_jugadores.num + 1], 
								lista_partidas.partida[l].baraja[2 * lista_partidas.partida[l].lista_jugadores.num + 2]);
						for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
							write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
					}
					else if(lista_partidas.partida[l].estado == 'F') {
						lista_partidas.partida[l].estado = 'T';
						char mensaje[100];
						sprintf(mensaje, "9$%d/2/%d", ID_partida, lista_partidas.partida[l].baraja[2 * lista_partidas.partida[l].lista_jugadores.num + 3]);
						for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
							write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
					}
					else if(lista_partidas.partida[l].estado == 'T') {
						lista_partidas.partida[l].estado = 'R';
						char mensaje[100];
						sprintf(mensaje, "9$%d/3/%d", ID_partida, lista_partidas.partida[l].baraja[2 * lista_partidas.partida[l].lista_jugadores.num + 4]);
						for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
							write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
					}
					else if(lista_partidas.partida[l].estado == 'R') {
						lista_partidas.partida[l].estado = 'E';
						
						//COMPRUEBA COMBINACIONES DE CARTAS DE TO CRISTO QUE SIGA
						//GENERA SIGUIENTE RONDA!!!!!!!!
					}
					else
						printf("Error inesperado al pasar de fase.");
					pthread_mutex_unlock(&mutex);
					
					//Pasamos el turno al jugador de después del dealer
					int m = 0;
					encontrado = 0;
					while(m < lista_partidas.partida[l].lista_jugadores.num && !encontrado) {
						if(strcmp(lista_partidas.partida[l].lista_jugadores.usuario[m].nombre, lista_partidas.partida[l].dealer) == 0)
							encontrado = 1;
						else
							m++;
					}
					int o = m + 1;
					encontrado = 0;
					while(m != o && !encontrado) {
						if(lista_partidas.partida[l].lista_jugadores.usuario[o].estado == 'C' || lista_partidas.partida[l].lista_jugadores.usuario[o].estado == 'R' || lista_partidas.partida[l].lista_jugadores.usuario[o].estado == 'W')
							encontrado = 1;
						else if (m == lista_partidas.partida[l].lista_jugadores.num)
							o = 0;
						else
							o++;
					}
					char mensaje3[100];
					sprintf(mensaje3, "11$%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[m].nombre);
					EnviarMensaje(mensaje3, ID_partida);
				}
				else {
					//Pasamos el turno al siguiente jugador
					int m = k;
					if(m == lista_partidas.partida[l].lista_jugadores.num)
						m = 0;
					else
						m++;
					encontrado = 0;
					while(!encontrado && m != k) {
						if(lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'A' && lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'O' && lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'F')
							encontrado = 1;
						else if(m = lista_partidas.partida[l].lista_jugadores.num)
							m = 0;
						else
							m++;
					}
					if(encontrado) {
						char mensaje3[100];
						sprintf(mensaje3, "11$%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[m].nombre);
						EnviarMensaje(mensaje3, ID_partida);
					}
					else
					   printf("Error inesperado al pasar turno.");
				}
			}
			else {
				//Pasamos el turno al siguiente jugador
				int m = k;
				if(m == lista_partidas.partida[l].lista_jugadores.num)
					m = 0;
				else
					m++;
				encontrado = 0;
				while(!encontrado && m != k) {
					if(lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'A' && lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'O' && lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'F')
						encontrado = 1;
					else if(m = lista_partidas.partida[l].lista_jugadores.num)
						m = 0;
					else
						m++;
				}
				if(encontrado) {
					char mensaje3[100];
					sprintf(mensaje3, "11$%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[m].nombre);
					EnviarMensaje(mensaje3, ID_partida);
				}
				else
				   printf("Error inesperado al pasar turno.");
			}
		}
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);
}

int main(int argc, char *argv[]){
	//Inicio del código
	
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
	serv_adr.sin_port = htons(50082);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen\n");
	
	pthread_t thread;
	
	
	// Atendemos peticiones
	for(;;) {
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexi?n\n");
		
		
		//Crear thead y decirle lo que tiene que hacer
		pthread_create (&thread, NULL, AtenderCliente, &sock_conn);

	}
}

