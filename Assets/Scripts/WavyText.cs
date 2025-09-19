using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class WavyText : MonoBehaviour
{
    [Header("Wave settings")]
    [SerializeField] private float amplitude = 5f;     // how tall the wave is (local units)
    [SerializeField] private float frequency = 2f;     // speed
    [SerializeField] private float waveSpacing = 0.5f; // phase offset per char

    [Header("Behaviour")]
    [SerializeField] private bool playOnStart = false; // start wave immediately

    public TMP_Text tmp;

    private TMP_TextInfo textInfo;
    private TMP_MeshInfo[] cachedMeshInfo; // original vertex positions (copied once)
    private bool isActive = false;

    public float waitTime = 0.5f;
    private float time = 0f;


    void Start()
    {
        // Force a mesh update so textInfo is populated, then cache original vertices
        tmp.ForceMeshUpdate();
        cachedMeshInfo = tmp.textInfo.CopyMeshInfoVertexData();
        textInfo = tmp.textInfo;

        if (playOnStart) isActive = true;

        
    }

    void Update()
    {
        if (!isActive) return;

        //time += Time.deltaTime;
       // if (time < waitTime) {
        //    return;
       // }

        // Refresh textInfo (in case text changed)
        tmp.ForceMeshUpdate();
        cachedMeshInfo = tmp.textInfo.CopyMeshInfoVertexData();
        textInfo = tmp.textInfo;

        // For each visible character, compute offset relative to cached/original verts
        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] sourceVerts = cachedMeshInfo[meshIndex].vertices;   // original baseline
            Vector3[] destVerts = textInfo.meshInfo[meshIndex].vertices; // mesh we will modify

            float wave = Mathf.Sin(Time.unscaledTime * frequency + i * waveSpacing) * amplitude;
            Vector3 offset = new Vector3(0f, wave, 0f);

            destVerts[vertexIndex + 0] = sourceVerts[vertexIndex + 0] + offset;
            destVerts[vertexIndex + 1] = sourceVerts[vertexIndex + 1] + offset;
            destVerts[vertexIndex + 2] = sourceVerts[vertexIndex + 2] + offset;
            destVerts[vertexIndex + 3] = sourceVerts[vertexIndex + 3] + offset;
        }

        // Push modified vertex arrays back to the meshes
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    public void StartWave() => isActive = true;

    public void EndWave()
    {
        isActive = false;
        RestoreOriginalVertices();
        time = 0f;
    }

    private void RestoreOriginalVertices()
    {
        if (cachedMeshInfo == null) return;

        tmp.ForceMeshUpdate();
        textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int meshIndex = textInfo.characterInfo[i].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            Vector3[] sourceVerts = cachedMeshInfo[meshIndex].vertices;
            Vector3[] destVerts = textInfo.meshInfo[meshIndex].vertices;

            destVerts[vertexIndex + 0] = sourceVerts[vertexIndex + 0];
            destVerts[vertexIndex + 1] = sourceVerts[vertexIndex + 1];
            destVerts[vertexIndex + 2] = sourceVerts[vertexIndex + 2];
            destVerts[vertexIndex + 3] = sourceVerts[vertexIndex + 3];
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            var meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            tmp.UpdateGeometry(meshInfo.mesh, i);
        }
    }

}