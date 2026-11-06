using System.Collections.Generic;
using Unity.LEGO.Behaviours.Actions;
using Unity.Tutorials.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.LEGO.Tutorials
{
    internal sealed class ElevatorPrefabCriterion : Criterion
    {
        [SerializeField] private GameObject m_PrefabParent;
        [SerializeField] private FutureObjectReference m_FutureReference;

        private readonly HashSet<int> existingInstances = new HashSet<int>();
        private GameObject instance;
        private Scene scene;

        public override void StartTesting()
        {
            base.StartTesting();
            scene = SceneManager.GetActiveScene();
            existingInstances.Clear();
            foreach (var elevator in FindInstances())
            {
                existingInstances.Add(elevator.GetInstanceID());
            }

            EditorApplication.hierarchyChanged -= UpdateCompletion;
            EditorApplication.hierarchyChanged += UpdateCompletion;
            UpdateCompletion();
        }

        public override void StopTesting()
        {
            EditorApplication.hierarchyChanged -= UpdateCompletion;
            base.StopTesting();
        }

        private void OnDisable()
        {
            EditorApplication.hierarchyChanged -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            if (instance && instance.scene == scene)
            {
                return true;
            }

            foreach (var elevator in FindInstances())
            {
                if (!existingInstances.Contains(elevator.GetInstanceID()))
                {
                    instance = elevator;
                    m_FutureReference.SceneObjectReference.Update(instance);
                    return true;
                }
            }

            return false;
        }

        private IEnumerable<GameObject> FindInstances()
        {
            if (!m_PrefabParent)
            {
                yield break;
            }

            foreach (var action in FindObjectsOfType<ElevatorAction>())
            {
                var elevator = action.gameObject;
                if (elevator.scene == scene &&
                    PrefabUtility.GetCorrespondingObjectFromSource(elevator) == m_PrefabParent)
                {
                    yield return elevator;
                }
            }
        }

        public override bool AutoComplete()
        {
            if (!m_PrefabParent)
            {
                return false;
            }

            Selection.activeObject = PrefabUtility.InstantiatePrefab(m_PrefabParent);
            UpdateCompletion();
            return IsCompleted;
        }

        protected override IEnumerable<FutureObjectReference> GetFutureObjectReferences()
        {
            if (m_FutureReference)
            {
                yield return m_FutureReference;
            }
        }
    }
}
