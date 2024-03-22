using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum StrangeAttractorType
{
    Lorenz = 0,
    Rossler,
}

public class PointBehavior_AnimationStrangeAttractor : PointBehavior
{
    // https://chaoticatmospheres.com/mathrules-strange-attractors
    // https://strange-attractors.org/#/gallery

    [SerializeField] private float attractorSpeed = 0.1f;
    public float AttractorSpeed { get { return attractorSpeed; } set { attractorSpeed = value; } }
    [SerializeField] private StrangeAttractorType type = StrangeAttractorType.Lorenz;
    public void SetType(StrangeAttractorType inType)
    {
        type = inType;
        switch (type)
        {
            case StrangeAttractorType.Lorenz:
            {
                AttractorFunction = Lorenz;
                SetParameters(10f, 28f, 8f / 3f);
                break;
            }
            case StrangeAttractorType.Rossler:
            {
                AttractorFunction = Rossler;
                SetParameters(0.2f, 0.2f, 5.7f);
                break;
            }
            default:
            {
                break;
            }
        }
    }
    private List<float> attractorParameters;
    public delegate Vector3 Vector3Delegate_Vector3(Vector3 inVector);
    public static event Vector3Delegate_Vector3 AttractorFunction;



    protected void Awake()
    {
        SetType(StrangeAttractorType.Lorenz);
    }

    //public override Vector2 UpdateBehavior(Vector2 inVector)
    //{
    //    // TODO: Handle this
    //    return inVector * 0.01f;
    //    //throw new System.NotImplementedException();
    //}

    public override Vector3 UpdateBehavior(List<Vector3> InPoints, int ListIndex = -1)
    {
        if (AttractorFunction != null)
            return AttractorFunction(InPoints[ListIndex]) * AttractorSpeed;

        return Vector3.zero;
    }

    private void SetParameters(params float[] parameters)
    {
        attractorParameters = new List<float>();
        for (int i = 0; i < parameters.Length; i++)
        {
            attractorParameters.Add(parameters[i]);
        }
    }

    private Vector3 Lorenz(Vector3 inVector)
    {
        // Domain is approx. 40 on greatest axis (double it since the base is [-0.5, 0.5])
        float scale = 80f;
        SetParameters(10f, 28f, 8f / 3f);

        Vector3 scaledPosition = scale * inVector;
        return new Vector3(
            attractorParameters[0] * scaledPosition.y - scaledPosition.x,
            scaledPosition.x * (attractorParameters[1] - scaledPosition.z),
            scaledPosition.x * scaledPosition.y - attractorParameters[2] * scaledPosition.z) / scale;
    }

    private Vector3 Rossler(Vector3 inVector)
    {
        return new Vector3(
            -(inVector.y + inVector.z),
            inVector.x + attractorParameters[0] * inVector.y,
            attractorParameters[1] + inVector.z * (inVector.x - attractorParameters[2]));
    }
}
