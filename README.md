# CogniBoost: Athlete Edition

¡Bienvenido a CogniBoost: Athlete Edition! Un juego de memoria dinámico y educativo diseñado para desafiar tu agilidad mental y familiarizarte con habilidades cognitivas clave para el rendimiento atlético. Encuentra los pares, supera tu puntuación y explora el poder del entrenamiento mental en el deporte.

**Juega la versión WebGL aquí:** [https://andrexr369.itch.io/cogniboost]

## Acerca del Juego

CogniBoost: Athlete Edition es un juego de memoria con un giro deportivo. El objetivo es simple: voltea las cartas y encuentra todas las parejas en el menor tiempo y con la menor cantidad de clics posible. Este proyecto busca no solo entretener, sino también destacar la importancia de las habilidades cognitivas en diversas disciplinas.

### Características Principales
*   **Desafío de Memoria Clásico:** Jugabilidad intuitiva y adictiva de encontrar pares.
*   **Niveles de Dificultad:**
    *   **Fácil:** Ideal para una introducción rápida.
    *   **Media:** Un reto equilibrado para jugadores intermedios.
    *   **Difícil:** Para aquellos que buscan el máximo desafío a su memoria.
*   **Sistema de Puntuación:** Mide tu rendimiento basado en el tiempo, los clics y los pares encontrados.
*   **Leaderboard Local:** Guarda tus mejores puntuaciones y compite por superarte. Permite ingresar un nombre de jugador (estilo arcade).
*   **Retroalimentación Audiovisual:** Efectos de sonido para interacciones clave (selección, par correcto, par incorrecto) y un efecto de partículas al ganar.
*   **Interfaz Adaptable:** El tablero de juego y su marco se ajustan dinámicamente al número de cartas.
*   **WebGL:** Juega directamente en tu navegador, sin necesidad de descargas.

## Desarrollo y Tecnologías

Este proyecto fue desarrollado como parte de [Menciona el propósito, ej: "una prueba técnica para Alternova" o "un proyecto personal de aprendizaje"].

*   **Motor de Juego:** Unity [Especifica tu versión, ej. 2021.3.15f1]
*   **Lenguaje:** C#
*   **Control de Versiones:** Git con Git LFS (Large File Storage) para assets binarios.
*   **Interfaz de Usuario (UI):** Unity UI, utilizando TextMeshPro para la renderización de texto.
*   **Configuración de Niveles:** Carga dinámica de la disposición de las cartas desde archivos JSON.
*   **Persistencia de Datos:**
    *   `PlayerPrefs` para almacenar la dificultad seleccionada por el usuario.
    *   Archivo JSON local para el leaderboard de puntuaciones.
*   **Pruebas Unitarias:** Implementación de pruebas de Edit Mode con el Unity Test Framework (NUnit) para validar:
    *   Lógica de cálculo de la puntuación (`GameManager.CalculateScore`).
    *   Validación de la estructura y contenido de los archivos JSON de configuración (`ConfigLoader.IsValidConfig`).

## Estructura del Proyecto y Scripts Clave

El proyecto está organizado para promover la modularidad y la separación de responsabilidades. Los scripts principales incluyen:

*   **`GameManager.cs`**: Orquestador central del juego. Gestiona el estado de la partida, la lógica de selección y emparejamiento de cartas, el temporizador, el cálculo de puntuaciones, la actualización de la UI del juego y la coordinación con otros sistemas.
*   **`GridManager.cs`**: Responsable de la creación y disposición visual de la grilla de cartas en la UI, basándose en la configuración cargada.
*   **`Card.cs`**: Define el comportamiento y estado de una carta individual (visible, oculta, emparejada, tipo).
*   **`ConfigLoader.cs`**: Carga y valida los archivos JSON que definen la configuración de cada nivel/dificultad.
*   **`CardTypeSO.cs`** y **`CardRegistrySO.cs`**: ScriptableObjects utilizados para definir los tipos de cartas (ID, sprite) y un registro central para acceder a ellos eficientemente.
*   **DTOs (Data Transfer Objects)**: Clases como `BlockData.cs` (y `GameConfig.cs` dentro de él), `LeaderboardEntry.cs` (y `LeaderboardData.cs`), y `ResultsData.cs` (y `ResultsWrapper.cs`) para estructurar los datos para la serialización/deserialización JSON.
*   **`MainMenuManager.cs`**: Gestiona la navegación y la UI de la escena del menú principal.
*   **`DifficultySelector.cs`**: Maneja la lógica de la escena de selección de dificultad, guardando la elección del jugador.
*   **`LeaderboardManager.cs`**: Administra la lógica del leaderboard: carga, guardado, adición de nuevas puntuaciones y ordenación.
*   **`LeaderboardUI.cs`** y **`LeaderboardRowUI.cs`**: Encargados de la presentación visual del leaderboard, incluyendo la instanciación dinámica de filas.

## Flujo del Videojuego

1.  **Menú Principal:** El jugador inicia en esta pantalla, con opciones para "Iniciar", "Créditos" (o "¿Quién me creo?"), y "Salir".
2.  **Selección de Dificultad:** Al seleccionar "Iniciar", se navega a una pantalla donde el jugador elige la dificultad (Fácil, Media, Difícil).
3.  **Inicio de la Partida:**
    *   La dificultad elegida se almacena usando `PlayerPrefs`.
    *   Se carga la escena principal del juego.
    *   El `GameManager` lee la dificultad desde `PlayerPrefs` y determina qué archivo JSON de configuración usar.
    *   El `ConfigLoader` procesa este JSON.
    *   El `GridManager` genera el tablero de cartas.
    *   Comienza la partida: el temporizador se activa y la UI de estadísticas se muestra.
4.  **Jugabilidad (Gameplay):**
    *   El jugador hace clic para voltear dos cartas.
    *   Se reproducen efectos de sonido para la selección.
    *   **Par Correcto:** Si las cartas coinciden, se marcan como emparejadas, se reproducen un sonido de éxito y su apariencia cambia (tinte de color).
    *   **Par Incorrecto:** Si no coinciden, se reproduce un sonido de error y las cartas se vuelven a ocultar tras un breve instante.
5.  **Fin de la Partida:**
    *   Al encontrar todos los pares, el juego concluye.
    *   Se activa un efecto de partículas de victoria.
    *   Aparece un panel para que el jugador ingrese su nombre (limitado, estilo arcade).
6.  **Leaderboard:**
    *   El nombre y la puntuación se registran en el leaderboard local (almacenado en un archivo JSON).
    *   Se muestra la pantalla del leaderboard con las puntuaciones más altas.
    *   Se ofrece un botón para "Jugar de Nuevo".
7.  **Jugar de Nuevo:**
    *   El `GameManager` reinicia el juego, limpiando el tablero y recargando la configuración basada en la última dificultad seleccionada (leída de `PlayerPrefs`).
8.  **Volver al Menú:** Durante la partida, existe una opción para regresar a la escena de selección de dificultad/menú principal.

## Cómo Ejecutar el Proyecto

1.  Clona este repositorio: `git clone [https://github.com/AndresOrdonez369/CogniBoost]`
2.  Asegúrate de tener Git LFS instalado. Si es la primera vez, ejecuta `git lfs install` en tu terminal. Al clonar un repositorio que ya usa LFS, los archivos LFS deberían descargarse automáticamente.
3.  Abre el proyecto con Unity Hub, usando la versión de Unity **
4.  La escena inicial debería ser la del menú principal (ej. `MainMenuScene.unity`). Ábrela y presiona el botón "Play" en el editor de Unity.
    
## Contribuciones

Este proyecto fue desarrollado como parte de una prueba técnica. Actualmente, no se buscan contribuciones externas de forma activa. Sin embargo, si tienes alguna sugerencia o encuentras algún error, siéntete libre de abrir un "Issue" en la pestaña correspondiente de este repositorio de GitHub.

---