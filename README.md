# CogniBoost: Athlete Edition ????

¡Bienvenido a CogniBoost: Athlete Edition! Un juego de memoria dinámico y educativo diseñado para desafiar tu agilidad mental y familiarizarte con habilidades cognitivas clave para el rendimiento atlético. Encuentra los pares, supera tu puntuación y aprende sobre el poder del entrenamiento mental.
## ?? Sobre el Juego

CogniBoost: Athlete Edition es un clásico juego de encontrar pares con un toque deportivo y educativo. El objetivo es simple: voltea las cartas y encuentra todas las parejas en el menor tiempo y con la menor cantidad de clics posible. A medida que avanzas, no solo pones a prueba tu memoria, sino que también puedes (opcional) aprender sobre la importancia de las habilidades cognitivas en el deporte.

### Características Principales
*   **Desafío de Memoria Clásico:** La jugabilidad adictiva de encontrar pares que todos conocemos y amamos.
*   **Niveles de Dificultad:** Elige entre Fácil, Media y Difícil para adaptar el reto a tu habilidad. Cada dificultad carga una configuración de tablero diferente.
*   **Sistema de Puntuación:** Compite por la mejor puntuación basada en el tiempo, los clics y los pares encontrados.
*   **Leaderboard Local:** ¡Guarda tus mejores hazañas y sigue mejorando! Incluye un sistema para ingresar tu nombre (estilo arcade).
*   **Retroalimentación Sonora y Visual:** Efectos de sonido para interacciones clave y efectos de partículas al ganar.
*   **Interfaz Dinámica:** El tablero de juego y su marco se ajustan al tamaño de la configuración de cartas cargada.
*   **Construido para WebGL:** Juega directamente en tu navegador.

## ??? Desarrollo y Tecnologías

Este proyecto fue desarrollado como parte de [Menciona el propósito, ej: "una prueba técnica para Alternova" o "un proyecto personal para explorar el desarrollo de juegos con Unity"].

*   **Motor de Juego:** Unity 202X.X.X (Especifica tu versión de Unity)
*   **Lenguaje de Programación:** C#
*   **Gestión de Archivos Grandes:** Git LFS (Large File Storage) para manejar assets binarios como texturas y audio.
*   **UI:** Unity UI con TextMeshPro para textos de alta calidad.
*   **Datos de Configuración:** Los niveles y la disposición de las cartas se cargan dinámicamente desde archivos JSON.
*   **Persistencia de Datos:** `PlayerPrefs` para guardar la dificultad seleccionada y un archivo JSON local para el leaderboard.
*   **Pruebas Unitarias:** Se implementaron pruebas de Edit Mode utilizando el Unity Test Framework (NUnit) para validar la lógica de cálculo de puntaje y la validación de la configuración del juego.

## ?? Estructura del Proyecto y Scripts Clave

El proyecto sigue una estructura organizada para separar responsabilidades:

*   **`GameManager.cs`**: Orquestador principal del juego. Maneja el estado de la partida, la lógica de selección de cartas, el cálculo de puntuación, el temporizador, la actualización de la UI del juego y la comunicación con otros managers.
*   **`GridManager.cs`**: Responsable de crear y poblar la grilla de cartas en la UI basándose en la configuración cargada desde el JSON.
*   **`Card.cs`**: Controla el comportamiento y el estado visual de una carta individual (boca arriba, boca abajo, emparejada).
*   **`ConfigLoader.cs`**: Encargado de leer y validar los archivos de configuración JSON que definen los tableros de juego.
*   **`CardTypeSO.cs` y `CardRegistrySO.cs`**: ScriptableObjects para definir los diferentes tipos de cartas (su ID y sprite visual) y un registro para acceder a ellos.
*   **DTOs (Data Transfer Objects)**: Clases simples como `BlockData.cs`, `GameConfig.cs`, `LeaderboardEntry.cs`, `LeaderboardData.cs`, `ResultsData.cs` para estructurar los datos leídos y guardados en JSON.
*   **`MainMenuManager.cs`**: Gestiona la navegación y la lógica de la interfaz de usuario del menú principal (Iniciar, Créditos, Salir).
*   **`DifficultySelector.cs`**: Maneja la lógica de la escena de selección de dificultad, guardando la elección del jugador en `PlayerPrefs`.
*   **`LeaderboardManager.cs`**: Administra la carga, guardado, adición de entradas y ordenación del leaderboard local.
*   **`LeaderboardUI.cs` y `LeaderboardRowUI.cs`**: Controlan la presentación visual del leaderboard, instanciando filas según sea necesario.

## ?? Flujo del Videojuego

1.  **Menú Principal:** El jugador es recibido con opciones para Iniciar el juego, ver Créditos o Salir.
2.  **Selección de Dificultad:** Al presionar "Iniciar", el jugador es llevado a una pantalla para seleccionar la dificultad (Fácil, Media, Difícil).
3.  **Inicio del Juego:**
    *   La dificultad seleccionada se guarda en `PlayerPrefs`.
    *   Se carga la escena del juego.
    *   El `GameManager` lee la dificultad de `PlayerPrefs` y selecciona el archivo JSON de configuración correspondiente.
    *   El `ConfigLoader` carga y valida el JSON.
    *   El `GridManager` crea el tablero de cartas según la configuración.
    *   El juego comienza: el temporizador corre y la UI de estadísticas se actualiza.
4.  **Jugabilidad:**
    *   El jugador selecciona dos cartas.
    *   Se reproducen efectos de sonido para la selección.
    *   Si las cartas forman un par, se marcan como emparejadas, suenan un efecto de éxito y cambian de color.
    *   Si no forman un par, suenan un efecto de error y se vuelven a ocultar después de un breve instante.
5.  **Fin del Juego:**
    *   Cuando todos los pares son encontrados, el juego termina.
    *   Se activa un efecto de partículas de victoria.
    *   Se muestra un panel para que el jugador ingrese su nombre (3 caracteres, estilo arcade).
6.  **Leaderboard:**
    *   El nombre y la puntuación se guardan en el leaderboard local (un archivo JSON).
    *   Se muestra la pantalla del leaderboard con los mejores puntajes.
    *   Desde el leaderboard, el jugador puede optar por "Jugar de Nuevo".
7.  **Jugar de Nuevo:**
    *   El `GameManager` reinicia el estado del juego, limpia la grilla y carga la configuración (basada en la última dificultad seleccionada en `PlayerPrefs`) para una nueva partida.
8.  **Volver al Menú:** Desde la escena del juego, hay una opción para regresar a la escena del menú principal/selección de dificultad.

## ?? Cómo Ejecutar el Proyecto

1.  Clona este repositorio.
2.  Asegúrate de tener Git LFS instalado (`git lfs install` una vez por máquina).
3.  Abre el proyecto con Unity Hub usando la versión de Unity [Especifica tu versión, ej. 2021.3.15f1].
4.  Abre la escena del menú principal (ej. `MainMenuScene.unity`) y presiona Play.
    *O abre la escena del juego directamente (ej. `MemoryGameScene.unity`) si quieres probar esa parte. Se cargará una dificultad por defecto si no se ha seleccionado una previamente.*

## ?? Contribuciones

Este proyecto fue desarrollado como [menciona el propósito]. Si bien no se buscan activamente contribuciones externas en este momento, si encuentras algún error o tienes sugerencias, ¡no dudes en abrir un "Issue" en este repositorio!

**Para contribuir (si el proyecto fuera abierto):**
1.  Haz un Fork del proyecto.
2.  Crea tu Feature Branch (`git checkout -b feature/AmazingFeature`).
3.  Haz Commit de tus cambios (`git commit -m 'Add some AmazingFeature'`).
4.  Haz Push a la Branch (`git push origin feature/AmazingFeature`).
5.  Abre un Pull Request.
