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
char lista_conectados [500];
//Estructura necesaria para el acceso excluyente
pthread_mutex_t mutex = PTHREAD_MUTEX_INITIALIZER;

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
	sprintf(consulta, "INSERT INTO JUGADOR VALUES (%d,'%s','%s',0);", i + 1, usuario, clave);
	err = mysql_query(conn, consulta);
	if (err!=0) {
		printf ("Error al introducir datos la base %u %s\n", 
		mysql_errno(conn), mysql_error(conn));
		return -3;
	}
	return 0;
}


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
		printf("Error. Los datos no coinciden.");
		return -2;
	}
	
	//Anadimos el nombre del usuario conectado a lista_conectados
	pthread_mutex_lock(&mutex);
	sprintf(lista_conectados, "%s%s/", lista_conectados, usuario);
	pthread_mutex_unlock(&mutex);
	
	return 0;
}


int PuntuacionPerdedores(char lista[100], MYSQL *conn) {
	MYSQL_RES *resultado;
	MYSQL_ROW row;	
	int err;
	// Estructura especial para almacenar resultados de consultas 
	char consulta [500];
	
	//SELECT  DISTINCT JUGADOR.PUNTUACION FROM (PARTIDA,JUGADOR,PARTICIPACION) 
	//WHERE	PARTIDA.GANADOR = 'Arnau'
	//AND  	PARTIDA.ID = PARTICIPACION.ID_P
	//AND 	PARTICIPACION.ID_J=JUGADOR.ID
	//AND 	JUGADOR.NOMBRE NOT IN ('Arnau');
	
	strcpy(consulta,"SELECT  DISTINCT JUGADOR.RECORD FROM (PARTIDA,JUGADOR,PARTICIPACION) WHERE PARTIDA.GANADOR = 'Arnau' AND  PARTIDA.ID = PARTICIPACION.ID_P AND PARTICIPACION.ID_J=JUGADOR.ID AND JUGADOR.NOMBRE NOT IN ('Arnau');");
	// consulta SQL para obtener una tabla con todos los datos
	// de la base de datos
	err=mysql_query (conn, consulta);
	if (err!=0) {
		printf ("Error al consultar datos de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		return -3;
	}
	
	resultado = mysql_store_result (conn);
	row = mysql_fetch_row (resultado);
	
	if (row == NULL) {
		printf ("No se han obtenido datos en la consulta\n");
		strcpy(lista, "Nadie ha perdido nunca contra Arnau.");
	}
	else
		while (row !=NULL) {
			sprintf(lista, "%s%s/", lista, row[0]);
			printf("%s\n", lista);
			row = mysql_fetch_row(resultado);
	}
		return 0;
}

int NombresPartidaLarga(char lista[100], MYSQL *conn) {
	int err;
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	
	char consulta[200];
	strcpy(consulta, "SELECT JUGADOR.NOMBRE FROM(PARTIDA, JUGADOR, PARTICIPACION) WHERE PARTIDA.DURACION = (SELECT MAX(DURACION) FROM PARTIDA) AND PARTIDA.ID = PARTICIPACION.ID_P AND PARTICIPACION.ID_J = JUGADOR.ID;");
	err = mysql_query(conn, consulta);
	
	if(err != 0)
	{
		printf ("Error al consultar datos de la base: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		return -2;
	}
	
	resultado = mysql_store_result(conn);
	
	row = mysql_fetch_row(resultado);
	strcpy(lista, "");
	if(row == NULL) {
		printf("No se han obtenido resultados en la consulta.\n");
	}
	else {
		while(row != NULL)
		{
			
			sprintf(lista, "%s%s/", lista, row[0]);
			row = mysql_fetch_row(resultado);
		}
	}
	return 0;
}

int DameRecord(MYSQL *conn) {
	
	MYSQL_RES *resultado;
	MYSQL_ROW row;
	int err;
	err = mysql_query(conn, "SELECT JUGADOR.ID FROM JUGADOR ORDER BY JUGADOR.RECORD DESC;");
	if(err != 0){
		printf("Error al consultar la base %u %s\n", mysql_errno(conn), mysql_error(conn));
		return -3;
	}
	
	resultado = mysql_store_result(conn);
	row = mysql_fetch_row(resultado);
	int ID;
	if(row == NULL)
		printf("No se ha obtenido datos en la consulta\n");
	else{
		ID = atoi(row[0]);
		return ID;
	}
	return 0;
}

void *AtenderCliente (void *socket)
{
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
	//inicializar la conexiï¿ƒï¾³n, entrando nuestras claves de acceso y
	//el nombre de la base de datos a la que queremos acceder 
	conn = mysql_real_connect (conn, "localhost","root", "mysql", "bd",0, NULL, 0);
	if (conn==NULL) {
		printf ("Error al inicializar la conexion: %u %s\n",
				mysql_errno(conn), mysql_error(conn));
		exit(1);
	}
	//int socket_conn = * (int *) socket;
	char buff[512];
	char buff2[512];
	int ret;
	
	int terminar = 0;
	while(terminar == 0) {
		// Ahora recibimos su nombre, que dejamos en buff
		ret=read(sock_conn,buff, sizeof(buff));
		printf ("Recibido\n");
		
		// Tenemos que anadirle la marca de fin de string 
		// para que no escriba lo que hay despues en el buffer
		buff[ret]='\0';
		
		//Escribimos el nombre en la consola
		
		printf ("Se ha conectado: %s\n",buff);
		
		
		char *p = strtok(buff, "/");
		int codigo =  atoi (p);
		if (codigo ==1)  {
			p = strtok(NULL, "/");
			char usuario[20];
			strcpy(usuario, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
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
		else if (codigo == 2) {
			p = strtok(NULL, "/");
			char usuario[20];
			strcpy(usuario, p);
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			int res = IniciarSesion(usuario, clave, conn);
			if(res ==0)
				strcpy (buff2, "Se ha iniciado sesion correctamente.");
			else if(res == -1)
				strcpy (buff2, "Error al consultar la base de datos.");
			else if(res == -2)
				strcpy (buff2, "Error. Los datos introducidos no coinciden.");
		}
		else if (codigo == 3) {
			char lista[100];
			strcpy(lista, "");
			int res = PuntuacionPerdedores(lista, conn);
			printf("%s\n", lista);
			if(res == 0)
				strcpy (buff2, lista);
		}
		else if (codigo == 4) {
			char lista[100];
			int res = NombresPartidaLarga(lista, conn);
			if(res == 0)
				strcpy (buff2, lista);
		}	
		else if (codigo == 5) {
			int res = DameRecord(conn);
			if(res > 0)
				sprintf (buff2, "%d", res);
		}
		//Entregar lista conectados
		else if (codigo == 6)
		{
			strcpy(buff2, lista_conectados);
			buff2[strlen(buff2)-1] = '\0';
		}
		//Desconexion: 0/nombre
		else if (codigo == 0) {
			
			p = strtok(NULL, "/");
			char nombre[20];
			strcpy(nombre, p);
			
			//Creamos una nueva lista SIN el ususario desconectado
			pthread_mutex_lock(&mutex);
			char *t;
			t = strtok(lista_conectados, "/");
			char nueva_lista[500];
			strcpy(nueva_lista, "");
			while(t != NULL)
			{
				if(strcmp(t, nombre) != 0)
					sprintf(nueva_lista, "%s%s/", nueva_lista, t);
				
				t = strtok(NULL, "/");
			}
			//Asignamos esta nueva lista a lista_conectados
			strcpy(lista_conectados, nueva_lista);
			pthread_mutex_unlock(&mutex);
			
			terminar=1;
		}
		printf ("%s\n", buff2);
		// Y lo enviamos
		write (sock_conn,buff2, strlen(buff2));
	}
	// Se acabo el servicio para este cliente
	close(sock_conn);
}

int main(int argc, char *argv[])
{
	int sock_conn, sock_listen, ret;
	struct sockaddr_in serv_adr;
	
	// INICIALITZACIONS
	// Obrim el socket
	if ((sock_listen = socket(AF_INET, SOCK_STREAM, 0)) < 0)
		printf("Error creant socket");
	// Fem el bind al port
	
	
	memset(&serv_adr, 0, sizeof(serv_adr));// inicialitza a zero serv_addr
	serv_adr.sin_family = AF_INET;
	
	// asocia el socket a cualquiera de las IP de la m?quina. 
	//htonl formatea el numero que recibe al formato necesario
	serv_adr.sin_addr.s_addr = htonl(INADDR_ANY);
	// escucharemos en el port 9050
	serv_adr.sin_port = htons(9020);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind");
	//La cola de peticiones pendientes no podr? ser superior a 4
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
		
		// Crear thead y decirle lo que tiene que hacer
		pthread_create (&thread, NULL, AtenderCliente, &sockets[i]);
		i=i+1;
	}
}

