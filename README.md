# CogniBoost: Athlete Edition

�Bienvenido a CogniBoost: Athlete Edition! Un juego de memoria din�mico y educativo dise�ado para desafiar tu agilidad mental y familiarizarte con habilidades cognitivas clave para el rendimiento atl�tico. Encuentra los pares, supera tu puntuaci�n y explora el poder del entrenamiento mental en el deporte.

**Juega la versi�n WebGL aqu�:** [https://andrexr369.itch.io/cogniboost]

## Acerca del Juego

CogniBoost: Athlete Edition es un juego de memoria con un giro deportivo. El objetivo es simple: voltea las cartas y encuentra todas las parejas en el menor tiempo y con la menor cantidad de clics posible. Este proyecto busca no solo entretener, sino tambi�n destacar la importancia de las habilidades cognitivas en diversas disciplinas.

### Caracter�sticas Principales
*   **Desaf�o de Memoria Cl�sico:** Jugabilidad intuitiva y adictiva de encontrar pares.
*   **Niveles de Dificultad:**
    *   **F�cil:** Ideal para una introducci�n r�pida.
    *   **Media:** Un reto equilibrado para jugadores intermedios.
    *   **Dif�cil:** Para aquellos que buscan el m�ximo desaf�o a su memoria.
*   **Sistema de Puntuaci�n:** Mide tu rendimiento basado en el tiempo, los clics y los pares encontrados.
*   **Leaderboard Local:** Guarda tus mejores puntuaciones y compite por superarte. Permite ingresar un nombre de jugador (estilo arcade).
*   **Retroalimentaci�n Audiovisual:** Efectos de sonido para interacciones clave (selecci�n, par correcto, par incorrecto) y un efecto de part�culas al ganar.
*   **Interfaz Adaptable:** El tablero de juego y su marco se ajustan din�micamente al n�mero de cartas.
*   **WebGL:** Juega directamente en tu navegador, sin necesidad de descargas.

## Desarrollo y Tecnolog�as

Este proyecto fue desarrollado como parte de una prueba t�cnica para Alternova.

*   **Motor de Juego:** Unity 6.1
*   **Lenguaje:** C#
*   **Control de Versiones:** Git con Git LFS (Large File Storage) para assets binarios.
*   **Interfaz de Usuario (UI):** Unity UI, utilizando TextMeshPro para la renderizaci�n de texto.
*   **Configuraci�n de Niveles:** Carga din�mica de la disposici�n de las cartas desde archivos JSON.
*   **Persistencia de Datos:**
    *   `PlayerPrefs` para almacenar la dificultad seleccionada por el usuario.
    *   Archivo JSON local para el leaderboard de puntuaciones.
*   **Pruebas Unitarias:** Implementaci�n de pruebas de Edit Mode con el Unity Test Framework (NUnit) para validar:
    *   L�gica de c�lculo de la puntuaci�n (`GameManager.CalculateScore`).
    *   Validaci�n de la estructura y contenido de los archivos JSON de configuraci�n (`ConfigLoader.IsValidConfig`).

## Estructura del Proyecto y Scripts Clave

El proyecto est� organizado para promover la modularidad y la separaci�n de responsabilidades. Los scripts principales incluyen:

*   **`GameManager.cs`**: Orquestador central del juego. Gestiona el estado de la partida, la l�gica de selecci�n y emparejamiento de cartas, el temporizador, el c�lculo de puntuaciones, la actualizaci�n de la UI del juego y la coordinaci�n con otros sistemas.
*   **`GridManager.cs`**: Responsable de la creaci�n y disposici�n visual de la grilla de cartas en la UI, bas�ndose en la configuraci�n cargada.
*   **`Card.cs`**: Define el comportamiento y estado de una carta individual (visible, oculta, emparejada, tipo).
*   **`ConfigLoader.cs`**: Carga y valida los archivos JSON que definen la configuraci�n de cada nivel/dificultad.
*   **`CardTypeSO.cs`** y **`CardRegistrySO.cs`**: ScriptableObjects utilizados para definir los tipos de cartas (ID, sprite) y un registro central para acceder a ellos eficientemente.
*   **DTOs (Data Transfer Objects)**: Clases como `BlockData.cs` (y `GameConfig.cs` dentro de �l), `LeaderboardEntry.cs` (y `LeaderboardData.cs`), y `ResultsData.cs` (y `ResultsWrapper.cs`) para estructurar los datos para la serializaci�n/deserializaci�n JSON.
*   **`MainMenuManager.cs`**: Gestiona la navegaci�n y la UI de la escena del men� principal.
*   **`DifficultySelector.cs`**: Maneja la l�gica de la escena de selecci�n de dificultad, guardando la elecci�n del jugador.
*   **`LeaderboardManager.cs`**: Administra la l�gica del leaderboard: carga, guardado, adici�n de nuevas puntuaciones y ordenaci�n.
*   **`LeaderboardUI.cs`** y **`LeaderboardRowUI.cs`**: Encargados de la presentaci�n visual del leaderboard, incluyendo la instanciaci�n din�mica de filas.

## Flujo del Videojuego

1.  **Men� Principal:** El jugador inicia en esta pantalla, con opciones para "Iniciar", "Cr�ditos" (o "�Qui�n me creo?"), y "Salir".
2.  **Selecci�n de Dificultad:** Al seleccionar "Iniciar", se navega a una pantalla donde el jugador elige la dificultad (F�cil, Media, Dif�cil).
3.  **Inicio de la Partida:**
    *   La dificultad elegida se almacena usando `PlayerPrefs`.
    *   Se carga la escena principal del juego.
    *   El `GameManager` lee la dificultad desde `PlayerPrefs` y determina qu� archivo JSON de configuraci�n usar.
    *   El `ConfigLoader` procesa este JSON.
    *   El `GridManager` genera el tablero de cartas.
    *   Comienza la partida: el temporizador se activa y la UI de estad�sticas se muestra.
4.  **Jugabilidad (Gameplay):**
    *   El jugador hace clic para voltear dos cartas.
    *   Se reproducen efectos de sonido para la selecci�n.
    *   **Par Correcto:** Si las cartas coinciden, se marcan como emparejadas, se reproducen un sonido de �xito y su apariencia cambia (tinte de color).
    *   **Par Incorrecto:** Si no coinciden, se reproduce un sonido de error y las cartas se vuelven a ocultar tras un breve instante.
5.  **Fin de la Partida:**
    *   Al encontrar todos los pares, el juego concluye.
    *   Se activa un efecto de part�culas de victoria.
    *   Aparece un panel para que el jugador ingrese su nombre (limitado, estilo arcade).
6.  **Leaderboard:**
    *   El nombre y la puntuaci�n se registran en el leaderboard local (almacenado en un archivo JSON).
    *   Se muestra la pantalla del leaderboard con las puntuaciones m�s altas.
    *   Se ofrece un bot�n para "Jugar de Nuevo".
7.  **Jugar de Nuevo:**
    *   El `GameManager` reinicia el juego, limpiando el tablero y recargando la configuraci�n basada en la �ltima dificultad seleccionada (le�da de `PlayerPrefs`).
8.  **Volver al Men�:** Durante la partida, existe una opci�n para regresar a la escena de selecci�n de dificultad/men� principal.

## C�mo Ejecutar el Proyecto

1.  Clona este repositorio: `git clone https://github.com/AndresOrdonez369/CogniBoost`
2.  Aseg�rate de tener Git LFS instalado. Si es la primera vez, ejecuta `git lfs install` en tu terminal. Al clonar un repositorio que ya usa LFS, los archivos LFS deber�an descargarse autom�ticamente.
3.  Abre el proyecto con Unity Hub, usando la versi�n de Unity **
4.  La escena inicial deber�a ser la del men� principal (ej. `MainMenuScene.unity`). �brela y presiona el bot�n "Play" en el editor de Unity.
    
## Contribuciones

Este proyecto fue desarrollado como parte de una prueba t�cnica. Actualmente, no se buscan contribuciones externas de forma activa. Sin embargo, si tienes alguna sugerencia o encuentras alg�n error, si�ntete libre de abrir un "Issue" en la pesta�a correspondiente de este repositorio de GitHub.

---