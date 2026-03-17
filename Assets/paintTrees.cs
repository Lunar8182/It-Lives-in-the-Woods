using UnityEngine;

public class TerrainTreeConverter : MonoBehaviour
{
    public Terrain terrain;

    void Start()
    {
        TreeInstance[] trees = terrain.terrainData.treeInstances;
        GameObject[] treePrefabs = new GameObject[terrain.terrainData.treePrototypes.Length];

        // cache prefabs
        for (int i = 0; i < terrain.terrainData.treePrototypes.Length; i++)
        {
            treePrefabs[i] = terrain.terrainData.treePrototypes[i].prefab;
        }

        for (int i = 0; i < trees.Length; i++)
        {
            TreeInstance t = trees[i];
            Vector3 pos = Vector3.Scale(t.position, terrain.terrainData.size) + terrain.transform.position;
            GameObject tree = Instantiate(treePrefabs[t.prototypeIndex], pos, Quaternion.identity);
            tree.transform.localScale = Vector3.one * t.widthScale;
        }
    }
}