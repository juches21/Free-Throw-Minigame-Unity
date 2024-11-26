using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BasketballThrow : MonoBehaviour
{
    // Referencias a objetos de la escena
    public GameObject projectileObject; // Objeto del proyectil
    public Slider powerSlider;         // Barra de poder

    // Posiciones y físicas
    public Vector3 initialPosition;      // Posición inicial del proyectil
    public Rigidbody projectileRigidbody; // Rigidbody del proyectil
    public Transform target;            // Objetivo del lanzamiento
    public float timeToTarget = 2f;     // Tiempo que tarda en alcanzar el objetivo

    // Variables internas para el lanzamiento
    public Vector3 adjustedTargetPosition; // Posición ajustada del objetivo
    public Vector3 modifiedLaunchPosition; // Posición modificada según el rango
    public int adjustmentSteps;            // Cantidad de pasos de ajuste
    private float powerLevel;              // Nivel de poder calculado
    private bool isChargingPower = false;  // Indica si se está cargando poder

    private void Start()
    {
        // Guardar la posición inicial del proyectil y objetivo
        initialPosition = projectileObject.transform.position;
        adjustedTargetPosition = target.position;
    }

    private void Update()
    {
        // Inicia la carga de poder cuando se mantiene la barra espaciadora
        if (Input.GetKey(KeyCode.Space))
        {
            if (!isChargingPower) // Evita reiniciar la corrutina varias veces
            {
                isChargingPower = true;
                StartCoroutine("ChargePower");
            }
        }
        else
        {
            // Lanza el proyectil al soltar la barra espaciadora
            if (isChargingPower)
            {
                isChargingPower = false;
                StopCoroutine("ChargePower");
                AdjustTargetPosition();
                LaunchProjectile(projectileRigidbody, modifiedLaunchPosition, timeToTarget);
            }
        }

        // Reinicia el proyectil al presionar R
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetProjectile();
        }
    }

    private void ResetProjectile()
    {
        // Reinicia la posición y velocidad del proyectil
        projectileRigidbody.velocity = Vector3.zero;
        projectileObject.transform.position = initialPosition;
    }

    public void LaunchProjectile(Rigidbody projectile, Vector3 targetPosition, float timeToTarget)
    {
        // Calcular desplazamientos y velocidades necesarias para alcanzar el objetivo
        Vector3 startPosition = projectile.position;
        Vector3 displacement = targetPosition - startPosition;
        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);

        float gravity = Physics.gravity.y;
        float verticalVelocity = (displacement.y / timeToTarget) - (0.5f * gravity * timeToTarget);
        Vector3 horizontalVelocity = horizontalDisplacement / timeToTarget;

        // Combinar velocidades y aplicarlas al Rigidbody
        Vector3 launchVelocity = horizontalVelocity + Vector3.up * verticalVelocity;
        projectile.velocity = launchVelocity;

        // Reiniciar la barra de poder
        powerSlider.value = 0;
    }

    public void AdjustTargetPosition()
    {
        // Ajustar la posición de lanzamiento según el nivel de poder
        if (powerLevel >= 50 && powerLevel <= 60) adjustmentSteps = 5;
        if (powerLevel == 55) adjustmentSteps = 0;
        if (powerLevel >= 61 && powerLevel <= 70) adjustmentSteps = 20;
        if (powerLevel >= 71 && powerLevel <= 80) adjustmentSteps = 30;
        if (powerLevel > 80) adjustmentSteps = 40;
        if (powerLevel >= 35 && powerLevel <= 49) adjustmentSteps = 20;
        if (powerLevel >= 25 && powerLevel <= 34) adjustmentSteps = 30;
        if (powerLevel < 25) adjustmentSteps = 40;

        modifiedLaunchPosition = adjustedTargetPosition;

        // Realiza ajustes aleatorios en la posición
        for (int i = 0; i <= adjustmentSteps; i++)
        {
            int randomDirection = Random.Range(0, 3);
            if (randomDirection == 0) modifiedLaunchPosition += new Vector3(0.05f, 0, 0);
            if (randomDirection == 1) modifiedLaunchPosition += new Vector3(0, 0.05f, 0);
            if (randomDirection == 2) modifiedLaunchPosition += new Vector3(0, 0, 0.05f);
        }
    }

    IEnumerator ChargePower()
    {
        // Carga o descarga la barra de poder continuamente
        powerLevel = 0f;
        bool decreasingPower = false;

        while (true)
        {
            yield return new WaitForSeconds(0.01f);

            if (powerLevel >= 100) decreasingPower = true;
            if (powerLevel <= 0) decreasingPower = false;

            if (!decreasingPower) powerLevel += 1f;
            else powerLevel -= 1f;

            powerSlider.value = powerLevel;
        }
    }
}
