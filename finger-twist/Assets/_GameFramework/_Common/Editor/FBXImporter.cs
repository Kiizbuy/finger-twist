using UnityEditor;

namespace GameFramework.Common
{
    public class FBXImporter : AssetPostprocessor
    {
        public void OnPreprocessModel()
        {
            var modelImporter = (ModelImporter)assetImporter;

            modelImporter.globalScale = 1;
            modelImporter.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            modelImporter.importBlendShapes = false;
            modelImporter.importLights = false;
            modelImporter.importVisibility = false;
            modelImporter.importCameras = false;
            modelImporter.preserveHierarchy = false;
            modelImporter.sortHierarchyByName = true;
            modelImporter.isReadable = false;

            if (modelImporter.animationType == ModelImporterAnimationType.Generic)
            {
                modelImporter.skinWeights = ModelImporterSkinWeights.Custom;
                modelImporter.maxBonesPerVertex = 2;
                modelImporter.minBoneWeight = 0.5f;
            }
        }
    }
}
