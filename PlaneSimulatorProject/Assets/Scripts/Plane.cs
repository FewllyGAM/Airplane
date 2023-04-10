using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour
{
    public HUD hud;

    public float power;
    public float acceleration;
    public Rigidbody rb;

    public Transform aileron_r;
    public Transform aileron_l;
    public float aileronRInclination = 0;
    public float aileronLInclination = 0;

    public Transform rudder;
    public float rudderInclination = 0;

    public Transform elevator;
    public float elevatorsInclination = 0;

    public Transform flaps;
    public float flapsInclination = 0;

    public AnimationCurve lift;
    public AnimationCurve drag;

    public float scrollSensitivy = 5.0f;
    public float maxThrustForce = 60.0f;
    public float thrustForce;
    public float liftForce;
    public float dragForce;

    private void FixedUpdate()
    {
        CalculateForces();

        RotatePlane();

        RotateParts();

    }

    void CalculateForces()
    {
        dragForce = drag.Evaluate(-flapsInclination) * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z) + Mathf.Abs(rb.velocity.y));

        //Sustentação = Coeficiente * densidade / 2 * area * velocidade ao quadrado
        liftForce = lift.Evaluate(-flapsInclination) * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)) / 8.0f * AtmosphereController.instance.AirDensity(transform.position.y);
        //liftForce = lift.Evaluate(-flapsInclination) * AtmosphereController.instance.AirDensity(transform.position.y) / 2 * 10/*area?*/ * Mathf.Pow(Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z), 2) /*/ 7.5f*/;

        rb.AddForce(transform.forward * thrustForce * rb.mass, ForceMode.Force);

        rb.AddForce(transform.up * liftForce * rb.mass, ForceMode.Force);

        rb.AddForce(-transform.forward * dragForce * rb.mass, ForceMode.Force);
    }

    void RotatePlane()
    {
        transform.Rotate(elevatorsInclination * Time.fixedDeltaTime * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)) * ((elevatorsInclination < 0) ? 0.0075f : 0.02f), 0, 0);

        if (aileronLInclination > 0) transform.Rotate(0, 0, -aileronLInclination * Time.fixedDeltaTime * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)) * 0.018f);
        else transform.Rotate(0, 0, aileronRInclination * Time.fixedDeltaTime * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)) * 0.018f);

        transform.Rotate(0, rudderInclination * Time.fixedDeltaTime * (Mathf.Abs(rb.velocity.x) + Mathf.Abs(rb.velocity.z)) * 0.008f, 0);
    }

    void RotateParts()
    {
        flaps.localEulerAngles = new Vector3(flapsInclination, flaps.localRotation.y, flaps.localRotation.z);

        elevator.localEulerAngles = new Vector3(elevator.localRotation.x, 90, elevatorsInclination);

        aileron_l.localEulerAngles = new Vector3(aileronLInclination, aileron_l.localRotation.y, aileron_l.localRotation.z);
        aileron_r.localEulerAngles = new Vector3(aileronRInclination, aileron_r.localRotation.y, aileron_r.localRotation.z);

        rudder.localEulerAngles = new Vector3(rudder.localRotation.x, rudderInclination, rudder.localRotation.z);
    }

    void Update()
    {
        if (Input.mouseScrollDelta.y > 0) thrustForce = Mathf.Clamp(thrustForce + scrollSensitivy, 0, maxThrustForce);
        else if (Input.mouseScrollDelta.y < 0) thrustForce = Mathf.Clamp(thrustForce - scrollSensitivy, 0, maxThrustForce);

        #region Flaps
        if (Input.GetKey(KeyCode.G))
        {
            flapsInclination = Mathf.Clamp(flapsInclination + 15 * Time.deltaTime, -25, 0);           
        }
        else if (Input.GetKey(KeyCode.F))
        {
            flapsInclination = Mathf.Clamp(flapsInclination - 15 * Time.deltaTime, -25, 25);
        }
        //else
        //{
        //    if (flapsInclination > 0) flapsInclination = Mathf.Clamp(flapsInclination - 15 * Time.deltaTime, 0, 25);
        //    else flapsInclination = Mathf.Clamp(flapsInclination + 15 * Time.deltaTime, -25, 0);
        //}
        #endregion

        #region Elevators
        if (Input.GetKey(KeyCode.W))
        {
            elevatorsInclination = Mathf.Clamp(elevatorsInclination + 25 * Time.deltaTime, -25, 25);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            elevatorsInclination = Mathf.Clamp(elevatorsInclination - 25 * Time.deltaTime, -25, 25);
        }
        else
        {
            if (elevatorsInclination > 0) elevatorsInclination = Mathf.Clamp(elevatorsInclination - 30 * Time.deltaTime, 0, 25);
            else elevatorsInclination = Mathf.Clamp(elevatorsInclination + 30 * Time.deltaTime, -25, 0);
        }
        #endregion

        #region Rudder&Ailerons
        if (Input.GetKey(KeyCode.A))
        {
            aileronLInclination = Mathf.Clamp(aileronLInclination - 25 * Time.deltaTime, -25, 25);
            aileronRInclination = Mathf.Clamp(aileronRInclination + 25 * Time.deltaTime, -25, 25);

            //rudderInclination = Mathf.Clamp(rudderInclination - 25 * Time.deltaTime, -25, 25);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            aileronRInclination = Mathf.Clamp(aileronRInclination - 25 * Time.deltaTime, -25, 25);
            aileronLInclination = Mathf.Clamp(aileronLInclination + 25 * Time.deltaTime, -25, 25);

            //rudderInclination = Mathf.Clamp(rudderInclination + 25 * Time.deltaTime, -25, 25);
        }
        else
        {
            if (aileronLInclination > 0) aileronLInclination = Mathf.Clamp(aileronLInclination - 30 * Time.deltaTime, 0, 25);
            else aileronLInclination = Mathf.Clamp(aileronLInclination + 30 * Time.deltaTime, -25, 0);

            if (aileronRInclination > 0) aileronRInclination = Mathf.Clamp(aileronRInclination - 30 * Time.deltaTime, 0, 25);
            else aileronRInclination = Mathf.Clamp(aileronRInclination + 30 * Time.deltaTime, -25, 0);

            //if (rudderInclination > 0) rudderInclination = Mathf.Clamp(rudderInclination - 30 * Time.deltaTime, 0, 25);
            //else rudderInclination = Mathf.Clamp(rudderInclination + 30 * Time.deltaTime, -25, 0);
        }
        #endregion

        #region Rudder
        if (Input.GetKey(KeyCode.Q))
        {
            //aileronLInclination = Mathf.Clamp(aileronLInclination - 25 * Time.deltaTime, -25, 25);
            //aileronRInclination = Mathf.Clamp(aileronRInclination + 25 * Time.deltaTime, -25, 25);

            rudderInclination = Mathf.Clamp(rudderInclination - 25 * Time.deltaTime, -25, 25);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            //aileronRInclination = Mathf.Clamp(aileronRInclination - 25 * Time.deltaTime, -25, 25);
            //aileronLInclination = Mathf.Clamp(aileronLInclination + 25 * Time.deltaTime, -25, 25);

            rudderInclination = Mathf.Clamp(rudderInclination + 25 * Time.deltaTime, -25, 25);
        }
        else
        {
            //if (aileronLInclination > 0) aileronLInclination = Mathf.Clamp(aileronLInclination - 30 * Time.deltaTime, 0, 25);
            //else aileronLInclination = Mathf.Clamp(aileronLInclination + 30 * Time.deltaTime, -25, 0);

            //if (aileronRInclination > 0) aileronRInclination = Mathf.Clamp(aileronRInclination - 30 * Time.deltaTime, 0, 25);
            //else aileronRInclination = Mathf.Clamp(aileronRInclination + 30 * Time.deltaTime, -25, 0);

            if (rudderInclination > 0) rudderInclination = Mathf.Clamp(rudderInclination - 30 * Time.deltaTime, 0, 25);
            else rudderInclination = Mathf.Clamp(rudderInclination + 30 * Time.deltaTime, -25, 0);
        }
        #endregion
        //transform.Rotate(flapsInclination * Time.deltaTime, 0, 0);

        hud.UpdateThrust(thrustForce / maxThrustForce);
    }
}
