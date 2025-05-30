# CogniBoost: Athlete Edition ????

�Bienvenido a CogniBoost: Athlete Edition! Un juego de memoria din�mico y educativo dise�ado para desafiar tu agilidad mental y familiarizarte con habilidades cognitivas clave para el rendimiento atl�tico. Encuentra los pares, supera tu puntuaci�n y aprende sobre el poder del entrenamiento mental.
## ?? Sobre el Juego

CogniBoost: Athlete Edition es un cl�sico juego de encontrar pares con un toque deportivo y educativo. El objetivo es simple: voltea las cartas y encuentra todas las parejas en el menor tiempo y con la menor cantidad de clics posible. A medida que avanzas, no solo pones a prueba tu memoria, sino que tambi�n puedes (opcional) aprender sobre la importancia de las habilidades cognitivas en el deporte.

### Caracter�sticas Principales
*   **Desaf�o de Memoria Cl�sico:** La jugabilidad adictiva de encontrar pares que todos conocemos y amamos.
*   **Niveles de Dificultad:** Elige entre F�cil, Media y Dif�cil para adaptar el reto a tu habilidad. Cada dificultad carga una configuraci�n de tablero diferente.
*   **Sistema de Puntuaci�n:** Compite por la mejor puntuaci�n basada en el tiempo, los clics y los pares encontrados.
*   **Leaderboard Local:** �Guarda tus mejores haza�as y sigue mejorando! Incluye un sistema para ingresar tu nombre (estilo arcade).
*   **Retroalimentaci�n Sonora y Visual:** Efectos de sonido para interacciones clave y efectos de part�culas al ganar.
*   **Interfaz Din�mica:** El tablero de juego y su marco se ajustan al tama�o de la configuraci�n de cartas cargada.
*   **Construido para WebGL:** Juega directamente en tu navegador.

## ??? Desarrollo y Tecnolog�as

Este proyecto fue desarrollado como parte de [Menciona el prop�sito, ej: "una prueba t�cnica para Alternova" o "un proyecto personal para explorar el desarrollo de juegos con Unity"].

*   **Motor de Juego:** Unity 202X.X.X (Especifica tu versi�n de Unity)
*   **Lenguaje de Programaci�n:** C#
*   **Gesti�n de Archivos Grandes:** Git LFS (Large File Storage) para manejar assets binarios como texturas y audio.
*   **UI:** Unity UI con TextMeshPro para textos de alta calidad.
*   **Datos de Configuraci�n:** Los niveles y la disposici�n de las cartas se cargan din�micamente desde archivos JSON.
*   **Persistencia de Datos:** `PlayerPrefs` para guardar la dificultad seleccionada y un archivo JSON local para el leaderboard.
*   **Pruebas Unitarias:** Se implementaron pruebas de Edit Mode utilizando el Unity Test Framework (NUnit) para validar la l�gica de c�lculo de puntaje y la validaci�n de la configuraci�n del juego.

## ?? Estructura del Proyecto y Scripts Clave

El proyecto sigue una estructura organizada para separar responsabilidades:

*   **`GameManager.cs`**: Orquestador principal del juego. Maneja el estado de la partida, la l�gica de selecci�n de cartas, el c�lculo de puntuaci�n, el temporizador, la actualizaci�n de la UI del juego y la comunicaci�n con otros managers.
*   **`GridManager.cs`**: Responsable de crear y poblar la grilla de cartas en la UI bas�ndose en la configuraci�n cargada desde el JSON.
*   **`Card.cs`**: Controla el comportamiento y el estado visual de una carta individual (boca arriba, boca abajo, emparejada).
*   **`ConfigLoader.cs`**: Encargado de leer y validar los archivos de configuraci�n JSON que definen los tableros de juego.
*   **`CardTypeSO.cs` y `CardRegistrySO.cs`**: ScriptableObjects para definir los diferentes tipos de cartas (su ID y sprite visual) y un registro para acceder a ellos.
*   **DTOs (Data Transfer Objects)**: Clases simples como `BlockData.cs`, `GameConfig.cs`, `LeaderboardEntry.cs`, `LeaderboardData.cs`, `ResultsData.cs` para estructurar los datos le�dos y guardados en JSON.
*   **`MainMenuManager.cs`**: Gestiona la navegaci�n y la l�gica de la interfaz de usuario del men� principal (Iniciar, Cr�ditos, Salir).
*   **`DifficultySelector.cs`**: Maneja la l�gica de la escena de selecci�n de dificultad, guardando la elecci�n del jugador en `PlayerPrefs`.
*   **`LeaderboardManager.cs`**: Administra la carga, guardado, adici�n de entradas y ordenaci�n del leaderboard local.
*   **`LeaderboardUI.cs` y `LeaderboardRowUI.cs`**: Controlan la presentaci�n visual del leaderboard, instanciando filas seg�n sea necesario.

## ?? Flujo del Videojuego

1.  **Men� Principal:** El jugador es recibido con opciones para Iniciar el juego, ver Cr�ditos o Salir.
2.  **Selecci�n de Dificultad:** Al presionar "Iniciar", el jugador es llevado a una pantalla para seleccionar la dificultad (F�cil, Media, Dif�cil).
3.  **Inicio del Juego:**
    *   La dificultad seleccionada se guarda en `PlayerPrefs`.
    *   Se carga la escena del juego.
    *   El `GameManager` lee la dificultad de `PlayerPrefs` y selecciona el archivo JSON de configuraci�n correspondiente.
    *   El `ConfigLoader` carga y valida el JSON.
    *   El `GridManager` crea el tablero de cartas seg�n la configuraci�n.
    *   El juego comienza: el temporizador corre y la UI de estad�sticas se actualiza.
4.  **Jugabilidad:**
    *   El jugador selecciona dos cartas.
    *   Se reproducen efectos de sonido para la selecci�n.
    *   Si las cartas forman un par, se marcan como emparejadas, suenan un efecto de �xito y cambian de color.
    *   Si no forman un par, suenan un efecto de error y se vuelven a ocultar despu�s de un breve instante.
5.  **Fin del Juego:**
    *   Cuando todos los pares son encontrados, el juego termina.
    *   Se activa un efecto de part�culas de victoria.
    *   Se muestra un panel para que el jugador ingrese su nombre (3 caracteres, estilo arcade).
6.  **Leaderboard:**
    *   El nombre y la puntuaci�n se guardan en el leaderboard local (un archivo JSON).
    *   Se muestra la pantalla del leaderboard con los mejores puntajes.
    *   Desde el leaderboard, el jugador puede optar por "Jugar de Nuevo".
7.  **Jugar de Nuevo:**
    *   El `GameManager` reinicia el estado del juego, limpia la grilla y carga la configuraci�n (basada en la �ltima dificultad seleccionada en `PlayerPrefs`) para una nueva partida.
8.  **Volver al Men�:** Desde la escena del juego, hay una opci�n para regresar a la escena del men� principal/selecci�n de dificultad.

## ?? C�mo Ejecutar el Proyecto

1.  Clona este repositorio.
2.  Aseg�rate de tener Git LFS instalado (`git lfs install` una vez por m�quina).
3.  Abre el proyecto con Unity Hub usando la versi�n de Unity [Especifica tu versi�n, ej. 2021.3.15f1].
4.  Abre la escena del men� principal (ej. `MainMenuScene.unity`) y presiona Play.
    *O abre la escena del juego directamente (ej. `MemoryGameScene.unity`) si quieres probar esa parte. Se cargar� una dificultad por defecto si no se ha seleccionado una previamente.*

## ?? Contribuciones

Este proyecto fue desarrollado como [menciona el prop�sito]. Si bien no se buscan activamente contribuciones externas en este momento, si encuentras alg�n error o tienes sugerencias, �no dudes en abrir un "Issue" en este repositorio!

**Para contribuir (si el proyecto fuera abierto):**
1.  Haz un Fork del proyecto.
2.  Crea tu Feature Branch (`git checkout -b feature/AmazingFeature`).
3.  Haz Commit de tus cambios (`git commit -m 'Add some AmazingFeature'`).
4.  Haz Push a la Branch (`git push origin feature/AmazingFeature`).
5.  Abre un Pull Request.
