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
	int puntos;
	int as;
	char estado; //F = finished W = waiting, O = out of money
	char mano[100];
	int numCartas;
} TJugador;

typedef struct {
	TJugador usuario[100];
	int num;
} TListaJugadores;

typedef struct {
	int numero[52]; //11 = J, 12 = Q, K = 13, A = 1
	int palo[52];    //1 = picas, 2 = treboles, 3 = corazones, 4 = diamantes
	int repartidas;
} TBaraja;

typedef struct {
	int ID;
	TListaJugadores lista_jugadores;
	TBaraja baraja;
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

int GetIndexConectado(int socket) {
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

int Registrarse(char usuario[20], char clave[20], MYSQL *conn) {	
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
	sprintf(consulta, "INSERT INTO JUGADOR VALUES (%d,'%s','%s',0, 0);", i + 1, usuario, clave);
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

void EnviarMensajeChat(char mensaje[100], int ID) {
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
	//Esta funcion se usa al principio de cada ronda para barajar las cartas
	//Primero de todo, se reinician las variables relacionadas con la baraja de la anterior ronda
	for(int k = 0; k < 52; k++) {
		lista_partidas.partida[l].baraja.palo[k] = -1;
		lista_partidas.partida[l].baraja.numero[k] = -1;
	}
	lista_partidas.partida[l].baraja.repartidas = 0;
	
	//Se inicia un bucle en el que se extrae un numero entre 1 y 13 y un palo entre 1 y 4, 
	//se comprueba que esa combinacion no este en la lista y, si es asi, se asigna la combinacion
	int i = 0; 
	while(i < 52) {
		int testNum = (rand() % 13) + 1;
		int testPalo = (rand() % 4) + 1;
		int j = 0;
		int encontrado = 0;
		while (j < i && !encontrado) {
			if(lista_partidas.partida[l].baraja.numero[j] == testNum && lista_partidas.partida[l].baraja.palo[j] == testNum)
				encontrado = 1;
			else
				j++;
		}
		if(!encontrado) {
			lista_partidas.partida[l].baraja.numero[i] = testNum;
			lista_partidas.partida[l].baraja.palo[i] = testPalo;
			i++;
		}
	}
}

int getIndexPartida(int ID_partida) {
	//Esta funcion se encarga de encontrar el indice de la lista de partidas donde esta la partida con ID indicado como parametro
	int l = 0;
	int encontrado = 0;
	while (!encontrado && l < lista_partidas.num) {
		if(lista_partidas.partida[l].ID == ID_partida)
			encontrado = 1;
		else
			l++;
	}
	return l;
}

void pedirCarta(int l, int j, int carta) {
	if(lista_partidas.partida[l].baraja.numero[carta] == 1) {
		if(lista_partidas.partida[l].baraja.numero[carta] 
				+ lista_partidas.partida[l].lista_jugadores.usuario[j].puntos <= 21) 
		{
			lista_partidas.partida[l].lista_jugadores.usuario[j].as++;
			lista_partidas.partida[l].lista_jugadores.usuario[j].puntos += 11;
		}
		else
		   lista_partidas.partida[l].lista_jugadores.usuario[j].puntos += 1;
	}
	else if(lista_partidas.partida[l].baraja.numero[carta] > 10) {
		if(lista_partidas.partida[l].lista_jugadores.usuario[j].puntos 
				+ lista_partidas.partida[l].baraja.numero[carta] > 21 
				&& lista_partidas.partida[l].lista_jugadores.usuario[j].as > 0) {
			lista_partidas.partida[l].lista_jugadores.usuario[j].as--;
		}
		else
		   lista_partidas.partida[l].lista_jugadores.usuario[j].puntos += 10;
	}
	else {
		if(lista_partidas.partida[l].lista_jugadores.usuario[j].puntos 
				+ lista_partidas.partida[l].baraja.numero[carta] > 21 
				&& lista_partidas.partida[l].lista_jugadores.usuario[j].as > 0) {
			lista_partidas.partida[l].lista_jugadores.usuario[j].as--;
			lista_partidas.partida[l].lista_jugadores.usuario[j].puntos -= 10;
		}
		lista_partidas.partida[l].lista_jugadores.usuario[j].puntos += lista_partidas.partida[l].baraja.numero[carta];
	}
	if(lista_partidas.partida[l].lista_jugadores.usuario[j].puntos > 21)
		lista_partidas.partida[l].lista_jugadores.usuario[j].estado = 'F';
	sprintf(lista_partidas.partida[l].lista_jugadores.usuario[j].mano, "%s%d-%d/", lista_partidas.partida[l].lista_jugadores.usuario[j].mano, 
			lista_partidas.partida[l].baraja.numero[carta], lista_partidas.partida[l].baraja.palo[carta]);
	lista_partidas.partida[l].lista_jugadores.usuario[j].numCartas++;
}

void terminarRonda(int l, int ID_partida) {
	char mensaje[200];
	strcpy(mensaje, "Todos los jugadores han terminado su turno.");
	EnviarMensajeChat(mensaje, ID_partida);
	char mensaje0[200];
	if(lista_partidas.partida[l].lista_jugadores.usuario[0].puntos > 21) {
		strcpy(mensaje0, "La banca se ha pasado de puntos.");
		EnviarMensajeChat(mensaje0, ID_partida);
	}
	else {
		sprintf(mensaje0, "La banca tiene %d puntos.", lista_partidas.partida[l].lista_jugadores.usuario[0].puntos);
		EnviarMensajeChat(mensaje0, ID_partida);
	}
	
	for(int i = 1; i < lista_partidas.partida[l].lista_jugadores.num; i++) {
		if(lista_partidas.partida[l].lista_jugadores.usuario[i].jugado == 0) {
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/0/0", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
		else if(lista_partidas.partida[l].lista_jugadores.usuario[i].puntos > 21 && lista_partidas.partida[l].lista_jugadores.usuario[0].puntos > 21) {
			char mensaje1[200];
			sprintf(mensaje1, "%s tambien se ha pasado de puntos. Recpuera su apuesta.", lista_partidas.partida[l].lista_jugadores.usuario[i].nombre);
			EnviarMensajeChat(mensaje1, ID_partida);
			
			lista_partidas.partida[l].lista_jugadores.usuario[i].jugado = 0;
			
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/%d/%d/%s", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas,  
					lista_partidas.partida[l].lista_jugadores.usuario[i].puntos, lista_partidas.partida[l].lista_jugadores.usuario[i].numCartas, 
					lista_partidas.partida[l].lista_jugadores.usuario[i].mano);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
		else if(lista_partidas.partida[l].lista_jugadores.usuario[i].puntos > 21) {
			char mensaje1[200];
			sprintf(mensaje1, "%s se ha pasado de puntos. Pierde su apuesta.", lista_partidas.partida[l].lista_jugadores.usuario[i].nombre);
			EnviarMensajeChat(mensaje1, ID_partida);
			
			lista_partidas.partida[l].lista_jugadores.usuario[i].fichas -= lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[0].fichas += lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[i].jugado = 0;
			
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/%d/%d/%s", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas,  
					lista_partidas.partida[l].lista_jugadores.usuario[i].puntos, lista_partidas.partida[l].lista_jugadores.usuario[i].numCartas, 
					lista_partidas.partida[l].lista_jugadores.usuario[i].mano);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
		else if(lista_partidas.partida[l].lista_jugadores.usuario[0].puntos > 21) {
			char mensaje1[200];
			sprintf(mensaje1, "%s tiene %d puntos. Recupera el doble de lo apostado.", 
					lista_partidas.partida[l].lista_jugadores.usuario[i].nombre, lista_partidas.partida[l].lista_jugadores.usuario[i].puntos);
			EnviarMensajeChat(mensaje1, ID_partida);
			
			lista_partidas.partida[l].lista_jugadores.usuario[i].fichas += lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[0].fichas -= lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[i].jugado = 0;
			
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/%d/%d/%s", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas,  
					lista_partidas.partida[l].lista_jugadores.usuario[i].puntos, lista_partidas.partida[l].lista_jugadores.usuario[i].numCartas, 
					lista_partidas.partida[l].lista_jugadores.usuario[i].mano);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
		else if(lista_partidas.partida[l].lista_jugadores.usuario[i].puntos == lista_partidas.partida[l].lista_jugadores.usuario[0].puntos) {
			char mensaje1[200];
			sprintf(mensaje1, "%s tiene %d puntos. Ha empatado con la banca. Recupera su apuesta.", 
					lista_partidas.partida[l].lista_jugadores.usuario[i].nombre, lista_partidas.partida[l].lista_jugadores.usuario[i].puntos);
			EnviarMensajeChat(mensaje1, ID_partida);
			
			lista_partidas.partida[l].lista_jugadores.usuario[i].jugado = 0;
			
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/%d/%d/%s", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas,  
					lista_partidas.partida[l].lista_jugadores.usuario[i].puntos, lista_partidas.partida[l].lista_jugadores.usuario[i].numCartas, 
					lista_partidas.partida[l].lista_jugadores.usuario[i].mano);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
		else if(lista_partidas.partida[l].lista_jugadores.usuario[i].puntos < lista_partidas.partida[l].lista_jugadores.usuario[0].puntos) {
			char mensaje1[200];
			sprintf(mensaje1, "%s tiene %d puntos. Tiene menos puntos que la banca. Pierde lo apostado.", 
					lista_partidas.partida[l].lista_jugadores.usuario[i].nombre, lista_partidas.partida[l].lista_jugadores.usuario[i].puntos);
			EnviarMensajeChat(mensaje1, ID_partida);
			
			lista_partidas.partida[l].lista_jugadores.usuario[i].fichas -= lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[0].fichas += lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[i].jugado = 0;
			
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/%d/%d/%s", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas,  
					lista_partidas.partida[l].lista_jugadores.usuario[i].puntos, lista_partidas.partida[l].lista_jugadores.usuario[i].numCartas, 
					lista_partidas.partida[l].lista_jugadores.usuario[i].mano);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
		else if(lista_partidas.partida[l].lista_jugadores.usuario[i].puntos > lista_partidas.partida[l].lista_jugadores.usuario[0].puntos) {
			char mensaje1[200];
			sprintf(mensaje1, "%s tiene %d puntos. Tiene mas puntos que la banca. Recupera el doble de lo apostado.", 
					lista_partidas.partida[l].lista_jugadores.usuario[i].nombre, lista_partidas.partida[l].lista_jugadores.usuario[i].puntos);
			EnviarMensajeChat(mensaje1, ID_partida);
			
			lista_partidas.partida[l].lista_jugadores.usuario[i].fichas += lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[0].fichas -= lista_partidas.partida[l].lista_jugadores.usuario[i].jugado;
			lista_partidas.partida[l].lista_jugadores.usuario[i].jugado = 0;
			
			char mensaje2[200];
			sprintf(mensaje2, "15$%d/%d/%d/%d/%d/%s", ID_partida, i, lista_partidas.partida[l].lista_jugadores.usuario[i].fichas,  
					lista_partidas.partida[l].lista_jugadores.usuario[i].puntos, lista_partidas.partida[l].lista_jugadores.usuario[i].numCartas, 
					lista_partidas.partida[l].lista_jugadores.usuario[i].mano);
			for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
				write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje2, strlen(mensaje2));
			printf("se ha enviado %s\n", mensaje2);
		}
	}
	
	sprintf(mensaje0, "15$%d/0/%d/%d/%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[0].fichas,  
			lista_partidas.partida[l].lista_jugadores.usuario[0].puntos, lista_partidas.partida[l].lista_jugadores.usuario[0].numCartas, 
			lista_partidas.partida[l].lista_jugadores.usuario[0].mano);
	for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
		write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje0, strlen(mensaje0));
	printf("se ha enviado %s\n", mensaje0);
	
	if(lista_partidas.partida[l].lista_jugadores.usuario[0].fichas <= 0) {
		terminarPartida();
	}
	else {
	    int j = 1;
		int encontrado = 0;
		while(!encontrado && j < lista_partidas.partida[l].lista_jugadores.num) {
			if(lista_partidas.partida[l].lista_jugadores.usuario[0].fichas > 0)
				encontrado = 1;
			else
				j++;
		}
		if(!encontrado)
			terminarPartida();
	}
	
	siguienteRonda(ID_partida, l);
}

void terminarPartida() {
	
}

void siguienteRonda(int ID_partida, int l) {
	char mensaje[100];
	strcpy(mensaje, "Comienza la siguiente ronda.");
	EnviarMensajeChat(mensaje, ID_partida);
	
	pthread_mutex_lock(&mutex);
	
	//Configuramos los parametros iniciales de cada jugador
	for(int k = 0; k < lista_partidas.partida[l].lista_jugadores.num; k++) {
		lista_partidas.partida[l].lista_jugadores.usuario[k].jugado = 0;
		lista_partidas.partida[l].lista_jugadores.usuario[k].puntos = 0;
		lista_partidas.partida[l].lista_jugadores.usuario[k].as = 0;
		lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'W';
		strcpy(lista_partidas.partida[l].lista_jugadores.usuario[k].mano, "");
		lista_partidas.partida[l].lista_jugadores.usuario[k].numCartas = 0;
	}
	
	//Barajamos las cartas
	BarajarCartas(l);
	
	//Repartimos las cartas. Cada jugador recibe las dos cartas que tiene en la mano inicialmente
	//A la vez, se suman los puntos que tienen las cartas que se envian al contador de cada cliente
	int j = 0;
	strcpy(mensaje, "");
	while(j < lista_partidas.partida[l].lista_jugadores.num) {
		pedirCarta(l, j, lista_partidas.partida[l].baraja.repartidas);
		lista_partidas.partida[l].baraja.repartidas++;
		pedirCarta(l, j, lista_partidas.partida[l].baraja.repartidas);
		lista_partidas.partida[l].baraja.repartidas++;
		sprintf(mensaje, "16$%d/%d/%d-%d/%d-%d", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[j].puntos,
				lista_partidas.partida[l].baraja.numero[lista_partidas.partida[l].baraja.repartidas - 2], 
				lista_partidas.partida[l].baraja.palo[lista_partidas.partida[l].baraja.repartidas - 2],
				lista_partidas.partida[l].baraja.numero[lista_partidas.partida[l].baraja.repartidas - 1], 
				lista_partidas.partida[l].baraja.palo[lista_partidas.partida[l].baraja.repartidas - 1]);
		write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
		printf("Se ha enviado %s\n", mensaje);
		
		j++;
	}
	pthread_mutex_unlock(&mutex);
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
				int i = GetIndexConectado(sock_conn);
				
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
			int i = GetIndexConectado(sock_conn);
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
				
				//Enviamos a todos los jugadores de la partida la lista de jugadores actualizada y los parámetros iniciales actualizados
				char mensaje2[200];
				strcpy(mensaje2, "");
				for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
					sprintf(mensaje2, "%s%s/", mensaje2, lista_partidas.partida[l].lista_jugadores.usuario[j].nombre);
				//eliminamos el ultimo caracter '/'
				mensaje2[strlen(mensaje2)-1] = '\0';
				char mensaje2final[200];
				sprintf(mensaje2final, "12$%d/%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.num, mensaje2);
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
			EnviarMensajeChat(mensaje, ID_partida);
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
			
			for(int j = k; j < lista_partidas.partida[j].lista_jugadores.num - 1; j++) {
				strcpy(lista_partidas.partida[j].lista_jugadores.usuario[k].nombre, lista_partidas.partida[j].lista_jugadores.usuario[k+1].nombre);
				lista_partidas.partida[j].lista_jugadores.usuario[k].sock = lista_partidas.partida[j].lista_jugadores.usuario[k+1].sock;
				lista_partidas.partida[j].lista_jugadores.usuario[k].estado = lista_partidas.partida[j].lista_jugadores.usuario[k+1].estado;
				lista_partidas.partida[j].lista_jugadores.usuario[k].as = lista_partidas.partida[j].lista_jugadores.usuario[k+1].as;
				lista_partidas.partida[j].lista_jugadores.usuario[k].fichas = lista_partidas.partida[j].lista_jugadores.usuario[k+1].fichas;
				lista_partidas.partida[j].lista_jugadores.usuario[k].jugado = lista_partidas.partida[j].lista_jugadores.usuario[k+1].jugado;
				lista_partidas.partida[j].lista_jugadores.usuario[k].numCartas = lista_partidas.partida[j].lista_jugadores.usuario[k+1].numCartas;
				lista_partidas.partida[j].lista_jugadores.usuario[k].puntos = lista_partidas.partida[j].lista_jugadores.usuario[k+1].puntos;
				strcpy(lista_partidas.partida[j].lista_jugadores.usuario[k].mano, lista_partidas.partida[j].lista_jugadores.usuario[k+1].mano);
			}
			lista_partidas.partida[j].lista_jugadores.num--;
			
			char mensaje0[100];
			sprintf(mensaje0, "7$%d/%d", ID_partida, k);
			for(int q = 0; q < lista_partidas.partida[j].lista_jugadores.num; q++)
				write(lista_partidas.partida[j].lista_jugadores.usuario[q].sock, mensaje0, strlen(mensaje0));
			printf("se ha enviado %s\n", mensaje0);
		}
		//El usuario quiere darse de baja de la base de datos
		//El servidor recibe 8/clave
		else if (codigo == 8) {
			p = strtok(NULL, "/");
			char clave[20];
			strcpy(clave, p);
			
			//Pedimos a la base de datos que nos proporcione la clave del usuario
			char consulta[200];
			strcpy(consulta, "SELECT JUGADOR.CONTRASENA FROM JUGADOR WHERE JUGADOR.NOMBRE='");
			strcat(consulta, lista_conectados.usuario[GetIndexConectado(sock_conn)].nombre);
			strcat(consulta, "'");
			int err = mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar la clave en la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				char respuesta[100];
				strcpy(respuesta, "8$-1");
				write(sock_conn, respuesta, strlen(respuesta));
			}
			else {
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				
				//Si la clave introducida por el usuario y la proporcionada por la base de datos no coinciden se envia un mensaje aclaratorio
				//NO se borra la cuenta
				if(strcmp(row[0], clave) != 0) {
					printf("Error. Clave mal introducida.\n");
					char respuesta[100];
					strcpy(respuesta, "8$-1");
					write(sock_conn, respuesta, strlen(respuesta));
				}
				//Si la clave es correcta se procede a eliminar el usuario
				else {
					char comando[200];
					sprintf(comando, "DELETE FROM JUGADOR WHERE JUGADOR.NOMBRE = '%s' AND JUGADOR.CONTRASENA = '%s'", lista_conectados.usuario[GetIndexConectado(sock_conn)].nombre, clave);
					err = mysql_query (conn, comando);
					if (err!=0) {
						//Si ha habido algun error en la eliminacion se envia mensaje de error al usuario
						printf ("Error al eliminar de la base %u %s\n", mysql_errno(conn), mysql_error(conn));
						char respuesta[100];
						strcpy(respuesta, "8$-1");
						write(sock_conn, respuesta, strlen(respuesta));
					}
					else {
						//Si todo ha ido bien, se envia confirmacion al usuario
						char respuesta[100];
						strcpy(respuesta, "8$0");
						write(sock_conn, respuesta, strlen(respuesta));
					}
				}
			}
		}
		//RESERVADO PARA CONSULTA A LA BASE DE DATOS 1
		//El servidor recibe ...
		else if (codigo == 9) {
			p = strtok(NULL, "/");
			char name[20];
			strcpy(name, p);
			
			//Pedimos a la base de datos que nos proporcione resultados del usuario
			char consulta[200];
			strcpy(consulta, "SELECT PARTIDA.GANADOR FROM(PARTIDA, JUGADOR, PARTICIPACION) WHERE PARTIDA.ID = PARTICIPACION.ID_P AND PARTICIPACION.ID_J = JUGADOR.ID AND JUGADOR.NOMBRE = '");
			strcat(consulta, name);
			strcat(consulta, "'");
			int err = mysql_query (conn, consulta);
			if (err!=0) {
				printf ("Error al consultar la clave en la base %u %s\n", mysql_errno(conn), mysql_error(conn));
				char respuesta[100];
				strcpy(respuesta, "9$-2"); //Se envia respuesta problemtica,devolvemos -2
				write(sock_conn, respuesta, strlen(respuesta));
			}
			else {
				MYSQL_RES *resultado;
				MYSQL_ROW row;
				resultado = mysql_store_result (conn);
				row = mysql_fetch_row (resultado);
				
				//Si la clave introducida por el usuario y la proporcionada por la base de datos no coinciden se envia un mensaje aclaratorio
				//NO se borra la cuenta
				if(row == NULL) {
					printf("No ha jugado con ese jugador.\n");
					char respuesta[100];
					strcpy(respuesta, "9$-1");//No ha jugado con ese jugador, devolvemos -1
					write(sock_conn, respuesta, strlen(respuesta));
				}
				//Si la clave es correcta se procede a eliminar el usuario
				else {
					char respuesta[400];
					char ganadores[300];
					strcpy(ganadores, "");
					
					int i=0;
					strcpy(respuesta, "9$0/");
					
					while (row != NULL){
						char var[20];
						strcpy(var, "");
						strcpy(var, row[0]);
						sprintf(ganadores, "%s%s/", ganadores, var);
						i++;
						row = mysql_fetch_row (resultado);
					}
					
					//eliminamos el ultimo caracter '/'
					ganadores[strlen(ganadores)-1] = '\0';
					
					sprintf(respuesta, "%s%d/%s", respuesta, i, ganadores);
					write(sock_conn, respuesta, strlen(respuesta));
					printf("%s", respuesta);
				}
			}
		}
		//RESERVADO PARA CONSULTA A LA BASE DE DATOS 2
		//El servidor recibe ...
		else if (codigo == 10) {
			
		}
		//RESERVADO PARA CONSULTA A LA BASE DE DATOS 3
		//El servidor recibe ...
		else if (codigo == 11) {
			
		}
		
		//Quiere recibir los datos de la partida (jugadores, fichas...)
		//El servidor recibe 12/id_partida
		else if (codigo == 12) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			
			//Buscamos la partida concreta en la lista de partidas
			int l = getIndexPartida(ID_partida);
			
			char mensaje2[200];
			strcpy(mensaje2, "");
			for(int j = 0; j < lista_partidas.partida[l].lista_jugadores.num; j++)
				sprintf(mensaje2, "%s%s/", mensaje2, lista_partidas.partida[l].lista_jugadores.usuario[j].nombre);
			//eliminamos el ultimo caracter '/'
			mensaje2[strlen(mensaje2)-1] = '\0';
			char mensaje2final[200];
			sprintf(mensaje2final, "12$%d/%d/%s", ID_partida, lista_partidas.partida[l].lista_jugadores.num, mensaje2);
			printf("Se ha enviado %s.\n", mensaje2final);
			write(sock_conn, mensaje2final, strlen(mensaje2final));
		}
		//Quiere comenzar una nueva partida
		//El servidor recibe 13/id_partida
		else if (codigo == 13) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			char mensaje[100];
			strcpy(mensaje, "Comienza la partida");
			EnviarMensajeChat(mensaje, ID_partida);
			
			//Buscamos la partida en la lista
			int l = getIndexPartida(ID_partida);
			
			pthread_mutex_lock(&mutex);
			
			//Configuramos los parametros iniciales de cada jugador
			for(int k = 0; k < lista_partidas.partida[l].lista_jugadores.num; k++) {
				lista_partidas.partida[l].lista_jugadores.usuario[k].fichas = 100;
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado = 0;
				lista_partidas.partida[l].lista_jugadores.usuario[k].puntos = 0;
				lista_partidas.partida[l].lista_jugadores.usuario[k].as = 0;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'W';
				strcpy(lista_partidas.partida[l].lista_jugadores.usuario[k].mano, "");
				lista_partidas.partida[l].lista_jugadores.usuario[k].numCartas = 0;
			}
			
			//Barajamos las cartas
			BarajarCartas(l);
			
			//Repartimos las cartas. Cada jugador recibe las dos cartas que tiene en la mano inicialmente
			//A la vez, se suman los puntos que tienen las cartas que se envian al contador de cada cliente
			int j = 0;
			strcpy(mensaje, "");
			while(j < lista_partidas.partida[l].lista_jugadores.num) {
				pedirCarta(l, j, lista_partidas.partida[l].baraja.repartidas);
				lista_partidas.partida[l].baraja.repartidas++;
				pedirCarta(l, j, lista_partidas.partida[l].baraja.repartidas);
				lista_partidas.partida[l].baraja.repartidas++;
				sprintf(mensaje, "13$%d/%d/%d-%d/%d-%d", ID_partida, lista_partidas.partida[l].lista_jugadores.usuario[j].puntos,
						lista_partidas.partida[l].baraja.numero[lista_partidas.partida[l].baraja.repartidas - 2], 
						lista_partidas.partida[l].baraja.palo[lista_partidas.partida[l].baraja.repartidas - 2],
						lista_partidas.partida[l].baraja.numero[lista_partidas.partida[l].baraja.repartidas - 1], 
						lista_partidas.partida[l].baraja.palo[lista_partidas.partida[l].baraja.repartidas - 1]);
				write (lista_partidas.partida[l].lista_jugadores.usuario[j].sock, mensaje, strlen(mensaje));
				printf("Se ha enviado %s\n", mensaje);
				
				j++;
			}
			pthread_mutex_unlock(&mutex);
		}
		//Quiere hacer alguna accion en la partida
		//El servidor recibe 14/id_partida/accion  Accion = 0/N cuando al principio apuesta N fichas, -1 cuando se retira, 1 cuando pide carta, 2 cuando se planta, 3 cuando dobla
		else if (codigo == 14) {
			p = strtok(NULL, "/");
			int ID_partida = atoi(p);
			p = strtok(NULL, "/");
			int accion = atoi(p);
			
			int l = getIndexPartida(ID_partida);
			
			//Buscamos al jugador en la lista de jugadores de la partida
			int k = 0;
			int encontrado = 0;
			while (!encontrado && k < lista_partidas.partida[l].lista_jugadores.num) {
				if(lista_partidas.partida[l].lista_jugadores.usuario[k].sock == sock_conn)
					encontrado = 1;
				else
					k++;
			}
			
			if (accion == 0) {
				//Apuesta inicial
				p = strtok(NULL, "/");
				int apuesta = atoi(p);
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado = apuesta;
				
				//Reenviamos la acción del jugador al resto de jugadores
				char mensaje0[100];
				sprintf(mensaje0, "14$%d/%d/0/%d", ID_partida, k, apuesta);
				for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
					write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje0, strlen(mensaje0));
				printf("se ha enviado %s\n", mensaje0);
				
				char mensaje[200];
				sprintf(mensaje, "%s ha apostado %d fichas.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre, apuesta);
				EnviarMensajeChat(mensaje, ID_partida);
			}
			else if (accion == -1) {
				//Se rinde
				lista_partidas.partida[l].lista_jugadores.usuario[k].fichas -= (lista_partidas.partida[l].lista_jugadores.usuario[k].jugado / 2);
				lista_partidas.partida[l].lista_jugadores.usuario[0].fichas += (lista_partidas.partida[l].lista_jugadores.usuario[k].jugado / 2);
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado = 0;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'F';
				
				//Reenviamos la acción del jugador al resto de jugadores
				char mensaje0[100];
				sprintf(mensaje0, "14$%d/%d/-1", ID_partida, k);
				for(int q = 0; q < lista_partidas.partida[l].lista_jugadores.num; q++)
					write(lista_partidas.partida[l].lista_jugadores.usuario[q].sock, mensaje0, strlen(mensaje0));
				printf("se ha enviado %s\n", mensaje0);
				
				char mensaje[200];
				sprintf(mensaje, "%s se ha rendido.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre);
				EnviarMensajeChat(mensaje, ID_partida);
			}
			else if (accion == 1) {
				//Pide otra carta
				pedirCarta(l, k, lista_partidas.partida[l].baraja.repartidas);
				char mensaje[100];
				//Enviamos 14$ID_partida/numJugador/1/puntos/carta
				sprintf(mensaje, "14$%d/%d/1/%d/%d-%d", ID_partida, k, lista_partidas.partida[l].lista_jugadores.usuario[k].puntos,
						lista_partidas.partida[l].baraja.numero[lista_partidas.partida[l].baraja.repartidas], 
						lista_partidas.partida[l].baraja.palo[lista_partidas.partida[l].baraja.repartidas]);
				lista_partidas.partida[l].baraja.repartidas++;
				write (lista_partidas.partida[l].lista_jugadores.usuario[k].sock, mensaje, strlen(mensaje));
				printf("Se ha enviado %s\n", mensaje);
				
				sprintf(mensaje, "%s ha pedido otra carta.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre);
				EnviarMensajeChat(mensaje, ID_partida);
			}
			else if (accion == 2) {
				//Se planta
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'F';
				
				char mensaje[200];
				sprintf(mensaje, "%s ha terminado su turno.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre);
				EnviarMensajeChat(mensaje, ID_partida);
			}
			else if (accion == 3) {
				//Dobla la apuesta y pide una ultima carta
				pedirCarta(l, k, lista_partidas.partida[l].baraja.repartidas);
				char mensaje[100];
				
				//Se envia 13$ID_partida/numJugador/3/puntos/carta
				sprintf(mensaje, "14$%d/%d/3/%d/%d-%d", ID_partida, k, lista_partidas.partida[l].lista_jugadores.usuario[k].puntos,
						lista_partidas.partida[l].baraja.numero[lista_partidas.partida[l].baraja.repartidas], 
						lista_partidas.partida[l].baraja.palo[lista_partidas.partida[l].baraja.repartidas]);
				lista_partidas.partida[l].baraja.repartidas++;
				write (lista_partidas.partida[l].lista_jugadores.usuario[k].sock, mensaje, strlen(mensaje));
				printf("Se ha enviado %s\n", mensaje);
				
				lista_partidas.partida[l].lista_jugadores.usuario[k].jugado = lista_partidas.partida[l].lista_jugadores.usuario[k].jugado * 2;
				lista_partidas.partida[l].lista_jugadores.usuario[k].estado = 'F';
				
				sprintf(mensaje, "%s ha doblado.", lista_partidas.partida[l].lista_jugadores.usuario[k].nombre);
				EnviarMensajeChat(mensaje, ID_partida);
			}
			
			int m = 0;
			encontrado = 0;
			while (!encontrado && m < lista_partidas.partida[l].lista_jugadores.num) {
				if(lista_partidas.partida[l].lista_jugadores.usuario[m].estado != 'F')
					encontrado = 1;
				else
					m++;
			}
			if(encontrado == 0)
				terminarRonda(l, ID_partida);
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
	serv_adr.sin_port = htons(50072);
	if (bind(sock_listen, (struct sockaddr *) &serv_adr, sizeof(serv_adr)) < 0)
		printf ("Error al bind\n");
	if (listen(sock_listen, 2) < 0)
		printf("Error en el Listen\n");
	
	pthread_t thread;
	
	
	// Atendemos peticiones
	for(;;) {
		printf ("Escuchando\n");
		
		sock_conn = accept(sock_listen, NULL, NULL);
		printf ("He recibido conexion\n");
		
		
		//Crear thead y decirle lo que tiene que hacer
		pthread_create (&thread, NULL, AtenderCliente, &sock_conn);

	}
}

