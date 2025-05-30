using NUnit.Framework; // Necesario para los atributos [Test], Assert, etc.
using UnityEngine;    // Para Mathf, si es necesario en las pruebas.


public class GameManagerScoreTests
{
    [Test]
    public void CalculateScore_ValidInputs_ReturnsCorrectPositiveScore()
    {
        // Arrange: Configura los datos de entrada y el resultado esperado.
        float time = 60f;
        int clicks = 10;
        int pairs = 5;
        int expectedScore = (5 * 1000) - (int)(60f * 5) - (10 * 10); // 4600

        // Act: Ejecuta el método que estás probando.
        // Como hicimos CalculateScore public static, podemos llamarlo directamente:
        int actualScore = GameManager.CalculateScore(time, clicks, pairs);

        // Assert: Verifica si el resultado actual es igual al esperado.
        Assert.AreEqual(expectedScore, actualScore, "El puntaje para entradas válidas no es el esperado.");
    }

    [Test]
    public void CalculateScore_HighPenalties_ReturnsZero()
    {
        // Arrange
        float time = 1000f;
        int clicks = 500;
        int pairs = 2;
        int expectedScore = 0; // Mathf.Max(0, resultadoNegativo) debe ser 0.

        // Act
        int actualScore = GameManager.CalculateScore(time, clicks, pairs);

        // Assert
        Assert.AreEqual(expectedScore, actualScore, "El puntaje con altas penalizaciones no se limitó a cero.");
    }

    [Test]
    public void CalculateScore_ZeroTimeZeroClicks_ReturnsMaxPairScore()
    {
        // Arrange
        float time = 0f;
        int clicks = 0;
        int pairs = 3;
        int expectedScore = 3 * 1000; // 3000

        // Act
        int actualScore = GameManager.CalculateScore(time, clicks, pairs);

        // Assert
        Assert.AreEqual(expectedScore, actualScore, "El puntaje sin penalizaciones no fue el máximo para los pares dados.");
    }
}