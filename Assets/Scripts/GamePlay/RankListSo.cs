using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace GamePlay
{
    [CreateAssetMenu(fileName = "RankList", menuName = "ScriptableObjects/RankList")]
    public class RankListSo : ScriptableObject
    {
        public SerializedDictionary<string, int> dict = new();
    }
}