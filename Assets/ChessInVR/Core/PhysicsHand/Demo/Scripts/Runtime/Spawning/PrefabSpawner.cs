using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using PhysicsHand.Structures;

namespace PhysicsHand.Demo.Spawning
{
    /// <summary>
    /// A component that spawns a prefab at a set interval with the ability to control many spawn settings.
    /// </summary>
    /// Author: Intuitive Gaming Solutions
    public class PrefabSpawner : MonoBehaviour
    {
        // SpawnPointMode.
        [Serializable]
        public enum SpawnPointMode
        {
            Random,  // Makes prefab instances spawn at a random spawn point.
            Linear,  // Makes prefab instances spawn in order starting at spawnTransforms[0].
            Reversed // Makes prefab instances spawn in reverse order starting at spawnTransforms[spawnTransforms.Length - 1].
        }

        // SpawnDelayMode.
        [Serializable]
        public enum SpawnDelayMode
        {
            Default,                // The next valid spawn time is based on the last time the spawner spawned something.
            ResetEveryDespawn       // The next valid spawn time resets everytime a spawned prefab becomes null.
        }

        // GameObjectEvent.
        [Serializable]
        public class GameObjectEvent : UnityEvent<GameObject> { }

        // PrefabSpawner.
        [Header("Instance")]
        [Tooltip("Is the spawner activated?")]
        public bool activated;

        [Header("Settings")]
        [Tooltip("The prefab to spawn.")]
        public GameObject prefab;
        [Tooltip("Should an instance of the prefab be spawned on Unity's Start() callback? (Will actually spawn in the first call of Update.)")]
        public bool spawnOnStart;
        [Tooltip("The maximum number of prefab instances that can exist in the world from this spawner at once. (0 = infinite)")]
        public int maxSpawnedPrefabs;
        [Tooltip("An array of possible spawn transforms.")]
        public Transform[] spawnTransforms;
        [Tooltip("(Optional) The parent for the spawned object.")]
        public Transform spawnInParent;
        [Tooltip("A range for the delay between prefab instantiations.")]
        public FloatMinMax spawnDelayRange;
        [Tooltip("The spawn delay mode determines how the next valid spawn time is chosen for this spawner.")]
        public SpawnDelayMode spawnDelayMode;
        [Tooltip("The mode for selecting which spawn point to use. (Random, Linear = in order, Reversed = reverse order)")]
        public SpawnPointMode spawnTransformSelectionMode;

        [Header("Events")]
        [Tooltip("An event that is invoked when the spawner instantiates a new prefab.\n\nArg0: GameObject - The GameObject involved in the event.")]
        public GameObjectEvent Spawned;
        [Tooltip("An event that is invoked whenever the spawn cleans up all instantiated prefabs that were spawned by it.")]
        public UnityEvent CleanedUp;


        /// <summary>The next Time.time that the spawner is allowed to instantiate a new prefab into the world.</summary>
        public float NextSpawnTime { get; set; }
        /// <summary>The next index of spawnTransforms[] to instantiate a prefab at.</summary>
        public int NextSpawnTransformIndex { get; set; }

        /// <summary>A list of all objects spawned into the world by this spawner.</summary>
        protected List<GameObject> m_SpawnedObjects = new List<GameObject>();

        // Unity callback(s).
        void Start()
        {
            if (!spawnOnStart)
            {
                // Reset next spawn time if we're not made to spawn one on Start().
                ResetNextSpawnTime();
            }
            else { ZeroNextSpawnTime(); }

            // Choose the first spawn position to use.
            NextSpawnTransformIndex = GetNextSpawnPointIndex();
        }

        void Update()
        {
            // Only 'think' while activated.
            if (activated)
            {
                // Remove any invalid spawned objects from the list.
                for (int i = m_SpawnedObjects.Count - 1; i >= 0; --i)
                {
                    if (m_SpawnedObjects[i] == null)
                    {
                        m_SpawnedObjects.RemoveAt(i);

                        // If the spawn delay mode is set to ResetEveryDespawn reset the next spawn time.
                        if (spawnDelayMode == SpawnDelayMode.ResetEveryDespawn)
                            ResetNextSpawnTime();
                    }
                }

                // Ensure that it's time to spawn a new object.
                if (Time.time >= NextSpawnTime)
                {
                    // Ensure we're still under the maximum number of spawned objects.
                    if (m_SpawnedObjects.Count + 1 <= maxSpawnedPrefabs)
                    {
                        // Spawn an object.
                        Spawn();
                    }
                }
            }
        }

        // Public method(s).
        /// <summary>
        /// Activates the spawner.
        /// </summary>
        public void Activate()
        {
            activated = true;
        }

        /// <summary>
        /// Deactives the spawner.
        /// </summary>
        public void Deactivate()
        {
            activated = false;
        }

        /// <summary>
        /// Destroys all prefab instances that have been spawned into the world by this spawner.
        /// </summary>
        public void Cleanup()
        {
            // Destroy all spawned objects.
            foreach (GameObject obj in m_SpawnedObjects)
            {
                Destroy(obj);
            }

            // Empty the spawned object list.
            m_SpawnedObjects.Clear();

            // Invoke the cleaned up event.
            CleanedUp?.Invoke();
        }

        /// <summary>
        /// Instantiates a new prefab into the world.
        /// </summary>
        public void Spawn()
        {
            // Instantiate a prefab instance.
            GameObject obj = Instantiate(prefab);
            if (obj != null)
            {
                // Set the object's position and euler angles if there are valid spawnTransforms, otherwise set it to the position of the prefab spawner..
                if (spawnTransforms.Length > 0)
                {
                    obj.transform.position = spawnTransforms[NextSpawnTransformIndex].position;
                    obj.transform.eulerAngles = spawnTransforms[NextSpawnTransformIndex].eulerAngles;
                }
                else { obj.transform.position = transform.position; }

                // Set parent.
                if (spawnInParent != null)
                    obj.transform.SetParent(spawnInParent, true);

                // Add the object to the spawned object list.
                m_SpawnedObjects.Add(obj);

                // Invoke the 'Spawned' event.
                Spawned?.Invoke(obj);
            }
            else { Debug.LogWarning("PrefabSpawner failed to spawn object! Object instantiation failed...", gameObject); }

            // Reset the next spawn time to delay spawning.
            ResetNextSpawnTime();

            // Move to next spawn point index.
            NextSpawnTransformIndex = GetNextSpawnPointIndex();
        }

        /// <summary>
        /// Registers a GameObject as if it were spawned by this PrefabSpawner even if it wasn't.
        /// NOTE: This DOES NOT invoke the 'Spawned' unity event.
        /// </summary>
        /// <param name="pObject"></param>
        public void RegisterSpawnedObject(GameObject pObject)
        {
            // Add the object to the spawned object list.
            m_SpawnedObjects.Add(pObject);
        }

        /// <summary>
        /// Deregisters all spawned objects so they are no longer managed by this component.
        /// NOTE: This DOES NOT clean up the spawned objects, it just dereferences them so the component no longer 'cares' about them.
        /// </summary>
        public void DeregisterAllSpawnedObjects()
        {
            // Clear the spawned objects list.
            m_SpawnedObjects.Clear();

            // If the spawn delay mode is set to ResetEveryDespawn reset the next spawn time. (NOTE: This is invoked manually since the objects are deregistered, not actually despawned.)
            if (spawnDelayMode == SpawnDelayMode.ResetEveryDespawn)
                ResetNextSpawnTime();
        }

        /// <summary>
        /// Resets the next spawn time so that it will take a full spawn delay before it the prefab spawner will spawn anything again.
        /// </summary>
        public void ResetNextSpawnTime()
        {
            // Update next spawn time.
            NextSpawnTime = GetNextSpawnTime();
        }

        /// <summary>
        /// Makes the next spawn time 0 effectively allowing the prefab spawner to spawn somethinig immediately (from a delay perspective).
        /// </summary>
        public void ZeroNextSpawnTime()
        {
            // Zero next spawn time.
            NextSpawnTime = 0;
        }

        // Protected method(s).
        /// <summary>
        /// Returns the next vlaid time that this spawner should beable to instantiate a new prefab.
        /// </summary>
        /// <returns>The next vlaid time that this spawner should beable to instantiate a new prefab.</returns>
        protected float GetNextSpawnTime()
        {
            return Time.time + UnityEngine.Random.Range(spawnDelayRange.minimum, spawnDelayRange.maximum);
        }

        /// <summary>
        /// A function that returns the next spawn point index based on the current spawn point index and the spawn transform selection mode.
        /// </summary>
        /// <returns>The next spawn point index that should be used.</returns>
        protected int GetNextSpawnPointIndex()
        {
            switch (spawnTransformSelectionMode)
            {
                case SpawnPointMode.Random:
                    // Select random point.
                    return UnityEngine.Random.Range(0, spawnTransforms.Length);
                case SpawnPointMode.Linear:
                    // Move to the next index, wrap if all spawn transforms have been used.
                    int next = NextSpawnTransformIndex + 1;
                    if (next >= spawnTransforms.Length)
                        next = 0;
                    return next;
                case SpawnPointMode.Reversed:
                    // Move to the next index in reverse, wrap to end of array if all spawn transforms have been used.
                    int last = NextSpawnTransformIndex - 1;
                    if (last < 0)
                        last = spawnTransforms.Length - 1;
                    return last;
                default:
                    Debug.LogWarning("No implementation for '" + spawnTransformSelectionMode.ToString() + "' in PrefabSpawner's 'GetNextSpawnPointIndex()' method.", gameObject);
                    return 0;
            }
        }
    }
}
