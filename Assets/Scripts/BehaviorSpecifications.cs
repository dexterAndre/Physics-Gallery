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
        Animate_Jitter,
        //Animate_Flocking,
        //Animate_VectorField,
        //Animate_WindSimulation,
        Animate_StrangeAttractor,
        //Animate_LotkaVolterra,
        //Animate_SpringSystem,
        // Overlay
        //Overlay_Web,
        //Overlay_Triangulation,
        //Overlay_ConvexHull,
        //Overlay_Voronoi,
        //Overlay_Duals,
        //Overlay_SpatialPartitioning,
        //Overlay_CenterOfMass,
        // Selection
        //Selector_ClosestToRay,
        //Selector_kMeansClustering,
        //Selector_PointSetRegistration,
    };
    #endregion

    #region Dictionaries
    // Functionless enum-name pairings
    private Dictionary<BoundsType, string> nameList_Bounds = new Dictionary<BoundsType, string>
    {
        { BoundsType.Square, "Square" },
        //{ BoundsType.Circle, "Circle" },
        //{ BoundsType.Sector, "Sector" },
        { BoundsType.Cube, "Cube" },
        //{ BoundsType.Sphere, "Sphere" },
        //{ BoundsType.Cone, "Cone" },
    };
    public Dictionary<BoundsType, string> NameList_Bounds { get { return nameList_Bounds; } }

    // Full function tables
    private Dictionary<EdgeResponse, BehaviorSpecification<VoidDelegate_Int>> edgeResponseMethods = new Dictionary<EdgeResponse, BehaviorSpecification<VoidDelegate_Int>>
    {
        { EdgeResponse.Overflow, new BehaviorSpecification<VoidDelegate_Int>("Overflow") },
        { EdgeResponse.Wrap, new BehaviorSpecification<VoidDelegate_Int>(
            ManagerPointSet.WrapPoint_Rectangular2D,
            ManagerPointSet.WrapPoint_Rectangular3D,
            "Wrap") },
        //{ EdgeResponse.Kill, new BehaviorSpecification<VoidDelegate_Int>(Manager_Lookup.Instance.ManagerPointSet.KillPoint, "Kill") },
        //{ EdgeResponse.Respawn, new BehaviorSpecification<VoidDelegate_Int>(Manager_Lookup.Instance.ManagerPointSet.RespawnPoint2D, Manager_Lookup.Instance.ManagerPointSet.RespawnPoint3D, "Respawn")},
    };
    public Dictionary<EdgeResponse, BehaviorSpecification<VoidDelegate_Int>> EdgeResponseMethods { get { return edgeResponseMethods; } }
    private Dictionary<GenerationMethod, BehaviorSpecification<VoidDelegate_ZeroParameters>> generationMethods = new Dictionary<GenerationMethod, BehaviorSpecification<VoidDelegate_ZeroParameters>>
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
    #endregion
}
