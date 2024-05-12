using System.Collections.Generic;
using UnityEngine;
using static Manager_PointSet;

[ExecuteAlways]
public class BehaviorSpecifications : MonoBehaviour
{
    private static Manager_PointSet ManagerPointSet { get { return Manager_Lookup.Instance.ManagerPointSet; } }

    #region Specifications
    [System.Serializable] public struct BehaviorSpecification<T>
    {
        [System.Serializable] public struct MethodNamed<T>
        {
            public T Method { get; }
            public string Name { get; }

            public MethodNamed(T InMethod, string InName)
            {
                Method = InMethod; Name = InName;
            }
        };

        public MethodNamed<T> Behavior2D { get; }
        public MethodNamed<T> Behavior3D { get; }

        public BehaviorSpecification(string InName)
        {
            Behavior2D = new MethodNamed<T>(default(T), InName);
            Behavior3D = new MethodNamed<T>(default(T), InName);
        }
        public BehaviorSpecification(string InName2D, string InName3D)
        {
            Behavior2D = new MethodNamed<T>(default(T), InName2D);
            Behavior3D = new MethodNamed<T>(default(T), InName3D);
        }
        public BehaviorSpecification(T InBehavior, string InName)
        {
            Behavior2D = new MethodNamed<T>(InBehavior, InName);
            Behavior3D = new MethodNamed<T>(InBehavior, InName);
        }
        public BehaviorSpecification(T InBehavior, string InName2D, string InName3D)
        {
            Behavior2D = new MethodNamed<T>(InBehavior, InName2D);
            Behavior3D = new MethodNamed<T>(InBehavior, InName3D);
        }
        public BehaviorSpecification(T InBehavior2D, T InBehavior3D, string InName)
        {
            Behavior2D = new MethodNamed<T>(InBehavior2D, InName);
            Behavior3D = new MethodNamed<T>(InBehavior3D, InName);
        }
        public BehaviorSpecification(T InBehavior2D, T InBehavior3D, string InName2D, string InName3D)
        {
            Behavior2D = new MethodNamed<T>(InBehavior2D, InName2D);
            Behavior3D = new MethodNamed<T>(InBehavior3D, InName3D);
        }

        public static List<string> MethodNames<T, U>(Dictionary<T, BehaviorSpecification<U>> InDictionary, bool Is2D)
        {
            List<string> names = new List<string>();
            foreach (KeyValuePair<T, BehaviorSpecification<U>> item in InDictionary)
            {
                names.Add(Is2D ? item.Value.Behavior2D.Name : item.Value.Behavior3D.Name);
            }

            return names;
        }
    };
    #endregion

    #region Enums
    [System.Serializable] public enum BoundsType
    {
        // 2D
        Square,
        //Circle,
        //Sector,

        // 3D
        Cube,
        //Sphere,
        //Cone
    };
    [System.Serializable] public enum EdgeResponse
    {
        Overflow,
        Wrap,
        //Kill,
        //Respawn
    };
    [System.Serializable] public enum GenerationMethod
    {
        Random,
        BlueNoise,
        LatticeRectangular,
        LatticeHexagonal,
        DoubleSlitDistribution,
        GaussianDistribution,
        Import,
    };
    [System.Serializable] public enum BehaviorMethod
    {
        // Animation
        Animation_Jitter,
        //Animation_Flocking,
        //Animation_VectorField,
        //Animation_WindSimulation,
        Animation_StrangeAttractor,
        //Animation_LotkaVolterra,
        //Animation_SpringSystem,
        // Overlay
        //Overlay_Web,
        //Overlay_Triangulation,
        //Overlay_ConvexHull,
        //Overlay_Voronoi,
        //Overlay_Duals,
        //Overlay_SpatialPartitioning,
        //Overlay_CenterOfMass,
        // Selection
        //Selection_ClosestToRay,
        //Selection_kMeansClustering,
        //Selection_PointSetRegistration,
    };
    #endregion

    #region Dictionaries
    // Enum-function lookup structs
    // Contains user-facing names and optionally methods
    // For both 2D and 3D versions (even if invalid)
    private static Dictionary<BoundsType, BehaviorSpecification<VoidDelegate_ZeroParameters>> boundsTypeMethods = new Dictionary<BoundsType, BehaviorSpecification<VoidDelegate_ZeroParameters>>
    {
        { BoundsType.Square, new BehaviorSpecification<VoidDelegate_ZeroParameters>("Square") },
        { BoundsType.Cube, new BehaviorSpecification<VoidDelegate_ZeroParameters>("Cube") },
    };
    public static Dictionary<BoundsType, BehaviorSpecification<VoidDelegate_ZeroParameters>> BoundsTypeMethods { get { return boundsTypeMethods; } }
    private static Dictionary<EdgeResponse, BehaviorSpecification<VoidDelegate_Int>> edgeResponseMethods = new Dictionary<EdgeResponse, BehaviorSpecification<VoidDelegate_Int>>
    {
        { EdgeResponse.Overflow, new BehaviorSpecification<VoidDelegate_Int>("Overflow") },
        { EdgeResponse.Wrap, new BehaviorSpecification<VoidDelegate_Int>(
            ManagerPointSet.WrapPoint_Rectangular2D,
            ManagerPointSet.WrapPoint_Rectangular3D,
            "Wrap") },
        //{ EdgeResponse.Kill, new BehaviorSpecification<VoidDelegate_Int>(Manager_Lookup.Instance.ManagerPointSet.KillPoint, "Kill") },
        //{ EdgeResponse.Respawn, new BehaviorSpecification<VoidDelegate_Int>(Manager_Lookup.Instance.ManagerPointSet.RespawnPoint2D, Manager_Lookup.Instance.ManagerPointSet.RespawnPoint3D, "Respawn")},
    };
    public static Dictionary<EdgeResponse, BehaviorSpecification<VoidDelegate_Int>> EdgeResponseMethods { get { return edgeResponseMethods; } }
    private static Dictionary<GenerationMethod, BehaviorSpecification<VoidDelegate_ZeroParameters>> generationMethods = new Dictionary<GenerationMethod, BehaviorSpecification<VoidDelegate_ZeroParameters>>
    {
        { GenerationMethod.Random, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_Random2D,
            ManagerPointSet.GeneratePoints_Random3D,
            "Random") },
        { GenerationMethod.BlueNoise, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_BlueNoise2D, 
            ManagerPointSet.GeneratePoints_BlueNoise3D, 
            "Blue Noise") },
        { GenerationMethod.LatticeRectangular, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_LatticeRectangular2D,
            ManagerPointSet.GeneratePoints_LatticeRectangular3D,
            "Lattice (Rectangular)") },
        // TODO: Check if possible to have a 3D version of this or just disable otherwise
        { GenerationMethod.LatticeHexagonal, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_LatticeHexagonal2D,
            default(VoidDelegate_ZeroParameters),
            "Lattice (Hexagonal)",
            "N/A") },
        { GenerationMethod.DoubleSlitDistribution, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_DoubleSlitDistribution,
            "Double Slit Distribution") },
        { GenerationMethod.GaussianDistribution, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_GaussianDistribution2D,
            ManagerPointSet.GeneratePoints_GaussianDistribution3D,
            "Gaussian Circle",
            "Gaussian Sphere") },
        { GenerationMethod.Import, new BehaviorSpecification<VoidDelegate_ZeroParameters>(
            ManagerPointSet.GeneratePoints_Import,
            "Import") }
    };
    public static Dictionary<GenerationMethod, BehaviorSpecification<VoidDelegate_ZeroParameters>> GenerationMethods { get { return generationMethods; } }
    #endregion
}
