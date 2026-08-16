using Assimp;
using Dazed.Vitals.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dazed.Vitals.Content
{
    public static class Chest
    {
        public const string ContentTableFileName = "contentTable.ct";
        /// <summary>
        /// Where key is an id, and value a relative path
        /// </summary>
        private readonly static Dictionary<string, string> keyToPath = [];
        private readonly static HashSet<string> paths = [];

        public static bool IdExists(string id) => keyToPath.ContainsKey(id);
        public static bool ValueExists(string value) => paths.Contains(value);
        public static void AddId(ContentIdentification cId, out bool successful)
        {
            if (DazedInstance.IsContextRuntime)
                throw new OutOfContextExecutionException();

            successful = false;
            if (IdExists(cId.ID))
                return;
            keyToPath.Add(cId.ID, cId.Path);
            paths.Add(cId.Path);
            successful = true;
        }
        public static void RemoveId(string id, out bool successful)
        {
            if (DazedInstance.IsContextRuntime)
                throw new OutOfContextExecutionException();

            successful = false;
            if (!IdExists(id))
                return;
            paths.Remove(keyToPath[id]);
            keyToPath.Remove(id);
            successful = true;
        }

        public static void LoadContentTable(SerializedContentTable cT)
        {
            if (DazedInstance.IsContextRuntime && keyToPath.Count != 0)
                throw new Exception("Loading a table while one is already set, this should not be the case.");
            keyToPath.Clear();
            for (int i = 0; i < cT.Paths.Length; i++)
            {
                keyToPath.Add(cT.Keys[i], cT.Paths[i]);
                paths.Add(cT.Paths[i]);
            }
        }

        public static SerializedContentTable SerializeContentTable()
        {
            if (DazedInstance.IsContextRuntime)
                throw new OutOfContextExecutionException();

            string[] keys;
            string[] paths;

            keys = keyToPath.Keys.ToArray();
            paths = keyToPath.Values.ToArray();

            return new(keys, paths);
        }

        //Written this way to make faulty IDs stand out with an exception.
        public static string GetRelativePath(string id) => keyToPath[id];

        public static AssimpContext AssimpContext { get; private set; }
        internal static void Initialize()
        {
            string path = PathUtils.BaseDirectoryCombine(ContentTableFileName);
            if (!File.Exists(path))
                throw new NullReferenceException();

            string content = File.ReadAllText(path);
            SerializedContentTable? contentTable = JsonConvert.DeserializeObject<SerializedContentTable>(content);

            if (contentTable == null)
                throw new NullReferenceException();

            LoadContentTable(contentTable);
            AssimpContext = new();
        }

        //May be inefficient, but its only executed once every now and then, so it's *fine*
        /// <summary>
        /// Not recommended to be executed many times as it is inefficient, only purpose is to tell GUI tools that it exists and has certain
        /// id bonded to it.
        /// </summary>
        /// <param name="relativePath">The path relative from the executable folder</param>
        /// <returns>Id of an existent path</returns>
        /// <exception cref="OutOfContextExecutionException"></exception>
        public static string GetId(string relativePath)
        {
            if (DazedInstance.IsContextRuntime)
                throw new OutOfContextExecutionException();

            string[] valuesArray = keyToPath.Values.ToArray();

            for (int i = 0; i < keyToPath.Count; i++)
            {
                if (valuesArray[i] == relativePath)
                {
                    string[] keysArray = keyToPath.Keys.ToArray();
                    return keysArray[i];
                }
            }
            return string.Empty;
        }
    }
}
