using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public enum StrangeAttractorType
{
    Lorenz = 0,
    Rossler,
}

public class PointBehavior_AnimationStrangeAttractor : PointBehavior_Animate
{
    // https://chaoticatmospheres.com/mathrules-strange-attractors
    // https://strange-attractors.org/#/gallery

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
    [SerializeField] private float attractorSpeed = 0.1f;
    public float AttractorSpeed { get { return attractorSpeed; } set { attractorSpeed = value; } }
    private List<float> attractorParameters;
    public delegate Vector3 Vector3Delegate_ZeroParameters(ref Vector3 inVector);
    public static event Vector3Delegate_ZeroParameters AttractorFunction;



    protected void Awake()
    {
        SetType(StrangeAttractorType.Lorenz);
    }

    public override Vector3 UpdateBehavior(Vector3 inVector)
    {
        if (AttractorFunction != null)
            return AttractorFunction(ref inVector) * AttractorSpeed;

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

    private Vector3 Lorenz(ref Vector3 inVector)
    {
        // TODO: Find a way to scale attractor down
        SetParameters(10f, 28f, 8f / 3f);

        return new Vector3(
            attractorParameters[0] * (inVector.y - inVector.x),
            inVector.x * (attractorParameters[1] - inVector.z),
            inVector.x * inVector.y - attractorParameters[2] * inVector.z);
    }
    private Vector3 Rossler(ref Vector3 inVector)
    {
        return new Vector3(
            -(inVector.y + inVector.z),
            inVector.x + attractorParameters[0] * inVector.y,
            attractorParameters[1] + inVector.z * (inVector.x - attractorParameters[2]));
    }
}
