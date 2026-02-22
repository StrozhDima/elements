using JetBrains.Annotations;
using UnityEngine;

namespace Elements.Level
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class SaveService : ISaveService
    {
        private const string SaveKey = "GameSave";

        void ISaveService.Save(GameSaveData data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }

        bool ISaveService.TryLoad(out GameSaveData data)
        {
            if (!PlayerPrefs.HasKey(SaveKey))
            {
                data = default;
                return false;
            }

            var json = PlayerPrefs.GetString(SaveKey);
            data = JsonUtility.FromJson<GameSaveData>(json);
            return true;
        }

        void ISaveService.Clear() => PlayerPrefs.DeleteKey(SaveKey);
    }
}