using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NN_Math;

public class RunnerCtrl : MonoBehaviour
{
    // Find or create neural network with a given player ID
    NN neuralNetwork;
    public int playerID;
    public bool dead;
    public int framesAlive;

    public void SetupRunner(int playerID)
    {
        this.playerID = playerID;
        neuralNetwork = new NN(playerID, 5, 10, 1);
    }

    float speed =5f;
    [Range(-.9f,.9f)]
    public float turnRotation;

    public void FixedUpdate()
    {
        if (!dead)
        {
            // The number of frames is used as the runner's fitness
            framesAlive++;

            // Get next rotation from NN
            float[] inputs = GetSurroundingDistances();
            turnRotation = neuralNetwork.GetOutputValue(inputs) - 0.5f;

            // Move player forward and rotate them
            transform.position += transform.forward * Time.deltaTime * speed;
            transform.rotation = GetNextRotation();
        }
    }

    // Get distance from walls in 5 directions and sigmoid these distances
    public float[] GetSurroundingDistances()
    {
        float[] distances = new float[5] { 10,10,10,10,10};

        RaycastHit hit;

        if(Physics.Linecast(transform.position, transform.position - (transform.right * 10), out hit, 1 << LayerMask.NameToLayer("Wall")))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            distances[0] = hit.distance;
        }
        if (Physics.Linecast(transform.position, transform.position - (transform.right * 7.07f) + (transform.forward * 7.07f), out hit, 1 << LayerMask.NameToLayer("Wall")))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            distances[1] = hit.distance;
        }
        if (Physics.Linecast(transform.position, transform.position + (transform.forward * 10), out hit, 1 << LayerMask.NameToLayer("Wall")))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            distances[2] = hit.distance;
        }
        if (Physics.Linecast(transform.position, transform.position + (transform.right * 7.07f) + (transform.forward * 7.07f), out hit, 1 << LayerMask.NameToLayer("Wall")))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            distances[3] = hit.distance;
        }
        if (Physics.Linecast(transform.position, transform.position + (transform.right * 10), out hit, 1 << LayerMask.NameToLayer("Wall")))
        {
            Debug.DrawLine(transform.position, hit.point, Color.red);
            distances[4] = hit.distance;
        }
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] -= 5f;
            distances[i] = Math.Sigmoid(distances[i]);
        }


        return distances;
       
    }

    // Get current rotation plus turn rotation
    public Quaternion GetNextRotation()
    {
        Vector3 newRot = transform.rotation.eulerAngles;
        newRot += new Vector3(0f, turnRotation * 6f, 0f);
        if (System.Double.IsNaN((double)newRot.y)) newRot = new Vector3(newRot.x, 0f, newRot.z);
        return Quaternion.Euler(newRot);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8 && !dead)
        {
            dead = true;
            GameObject.Find("Factory").GetComponent<RunnerFactory>().RunnerDies(framesAlive,neuralNetwork.nn_data);
        }
    }
}
