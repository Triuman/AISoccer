using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Creates fields and keeps them in the pool.
    /// Accepts the fields back when they are not in use.
    /// Sets ignoreCollision between all fields, so that none of them can interact with each other.
    /// </summary>
    public class FieldManager : MonoBehaviour
    {
        public Field FieldPrefab;

        private readonly List<Field> fieldsAvailable = new List<Field>();
        private readonly List<Field> fieldsInUse = new List<Field>();

        private void Awake()
        {
            FieldPrefab.gameObject.SetActive(false);
        }

        public void CreateFields(List<Agent> agents)
        {
            if (fieldsAvailable.Count < agents.Count)
            {
                InitNewFields(agents.Count - fieldsAvailable.Count);
            }
            for (int f = 0; f < agents.Count; f++)
            {
                fieldsInUse.Add(fieldsAvailable[f]);
                fieldsAvailable[f].SetAgent(agents[f]);
            }
            fieldsAvailable.RemoveRange(0, agents.Count);
        }

        private void InitNewFields(int count)
        {
            for (int nf = 0; nf < count; nf++)
            {
                var newField = Instantiate(FieldPrefab);
                newField.Init();
                foreach (var field in fieldsAvailable)
                {
                    IgnoreCollision(newField, field);
                }
                fieldsAvailable.Add(newField);
            }
        }

        public void StartSimulation()
        {
            foreach (var field in fieldsInUse)
            {
                field.gameObject.SetActive(true);
            }
        }
        public void EndSimulation()
        {
            foreach (var field in fieldsInUse)
            {
                field.gameObject.SetActive(false);
                fieldsAvailable.Add(field);
            }
            fieldsInUse.Clear();
        }

        private static void IgnoreCollision(Field field1, Field field2)
        {
            foreach (var collider1 in field1.colliders)
            {
                foreach (var collider2 in field2.colliders)
                {
                    Physics2D.IgnoreCollision(collider1, collider2, true);
                }
            }
        }
    }

}