using UnityEngine;
using TMPro;

[ExecuteAlways]
public class KavisliYazi : MonoBehaviour
{
    private TMP_Text yazi;
    public AnimationCurve kavisEgrisi = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1f), new Keyframe(1, 0));
    public float kavisGucu = 50f;

    void Update()
    {
        if (yazi == null) yazi = GetComponent<TMP_Text>();
        if (yazi == null) return;

        yazi.ForceMeshUpdate();
        TMP_TextInfo textInfo = yazi.textInfo;

        float minX = yazi.bounds.min.x;
        float maxX = yazi.bounds.max.x;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;

            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

            for (int j = 0; j < 4; j++)
            {
                float offset = (vertices[vertexIndex + j].x - minX) / (maxX - minX);
                // Harfleri eðriye göre yukarý kaldýrýyoruz
                vertices[vertexIndex + j].y += kavisEgrisi.Evaluate(offset) * kavisGucu;
            }
        }
        yazi.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}