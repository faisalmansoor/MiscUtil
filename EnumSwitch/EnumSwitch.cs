namespace EnumSwitch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EnumSwitch<T> where T : struct
    {
        private readonly Dictionary<T, Action> _caseMap = new Dictionary<T, Action>();
        private readonly T? _switchOn;

        public EnumSwitch()
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
            {
                string msg = String.Format("Type argument {0} must be an enum", enumType);
                throw new ArgumentException(msg);
            }
        }

        public EnumSwitch(T switchOn)
            : this()
        {
            _switchOn = switchOn;
        }

        public EnumSwitch<T> Case(T option, Action action)
        {
            if (_caseMap.Keys.Contains(option))
            {
                throw new ArgumentException(String.Format("Duplicate case label {0}", option), "option");
            }

            _caseMap.Add(option, action);

            return this;
        }

        public EnumSwitch<T> Case(IEnumerable<T> options, Action action)
        {
            foreach (var item in options)
            {
                Case(item, action);
            }

            return this;
        }

        public EnumSwitch<T> Ignore(params T[] options)
        {
            return Case(options, () => { });
        }

        public EnumSwitch<T> Ignore(T option)
        {
            return Case(option, () => { });
        }

        public void Execute()
        {
            if (!_switchOn.HasValue)
            {
                throw new InvalidOperationException("SwitchOn not provided, either provide SwitchOn in constructor or use Execute(switchOn)");
            }

            Execute(_switchOn.Value);
        }

        public void Execute(T switchOn)
        {
            Validate();

            Action action;
            if (_caseMap.TryGetValue(switchOn, out action))
            {
                action();
            }
            else
            {
                throw new NotImplementedException(String.Format("Case {0} is not handled by this switch statement", switchOn));
            }
        }

        private void Validate()
        {
            var possibleValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            var missingValues = possibleValues.Except(_caseMap.Keys);
            if (missingValues.Count() != 0)
            {
                string msg = "Switch statement didn't handle cases: " + String.Join(", ", missingValues);
                throw new NotImplementedException(msg);
            }
        }
    }
}