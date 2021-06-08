using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework.Dialogue
{
    [System.Serializable]
    public class Condition
    {
        [SerializeField] private Disjunction[] _and;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
        {
            return _and.All(dis => dis.Check(evaluators));
        }

        [System.Serializable]
        private class Disjunction
        {
            [SerializeField] private Predicate[] _or;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return _or.Any(predicate => predicate.Check(evaluators));
            }
        }

        [System.Serializable]
        private class Predicate
        {
            [SerializeField] private string predicate;
            [SerializeField] private string[] parameters;
            [SerializeField] private bool negate = false;

            public bool Check(IEnumerable<IPredicateEvaluator> evaluators)
            {
                return evaluators.Select(evaluator => evaluator.Evaluate(predicate, parameters))
                                 .Where(result => result != null)
                                 .All(result => result != negate);
            }
        }
    }
}
