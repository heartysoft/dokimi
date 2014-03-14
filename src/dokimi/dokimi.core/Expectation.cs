using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace dokimi.core
{
    public interface Expectation
    {
        void DescribeTo(SpecInfo spec, MessageFormatter formatter);
        void VerifyTo(object[] input, SpecInfo results, MessageFormatter formatter);
    }

    public class Expectation<T> : Expectation
    {
        public readonly Expression<Func<T, bool>> Expect;
        private readonly string _description;

        public Expectation(Expression<Func<T, bool>> expect, string description)
        {
            Expect = expect;
            _description = description;
        }

        public Expectation(Expression<Func<T, bool>> expect)
            :this(expect, null)
        {
        }

        public void DescribeTo(SpecInfo spec, MessageFormatter formatter)
        {
            spec.ReportExpectation(this);
        }

        public void VerifyTo(object[] input, SpecInfo results, MessageFormatter formatter)
        {
            try
            {
                verify(input);
                results.ReportExpectationPass(this);
            }
            catch (Exception e)
            {
                results.ReportExpectationFail(this, e);
            }
        }

        private void verify(object[] input)
        {
            var eventsOfType = input.OfType<T>();

            if (eventsOfType.Any(e => Expect.Compile()(e)))
                return;

            throw new ExpectationFailedException(this, input);
        }


        public virtual string GetPrettyPrintPrefix()
        {
            if (typeof (T).IsValueType)
                return string.Empty;

            var name = typeof (T).Name;

            return name.ToWords();
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_description) == false)
                return _description;

            var prefix = GetPrettyPrintPrefix();

            if (string.IsNullOrWhiteSpace(prefix))
                return GetPrettyPrintSuffix();

            var sb = new StringBuilder();
            sb.AppendFormat("{0} with ", prefix);
            sb.Append(GetPrettyPrintSuffix());

            return sb.ToString();
        }

        public string GetPrettyPrintSuffix()
        {
            var sb = new StringBuilder();
            if (Expect.Body.NodeType == ExpressionType.Equal)
            {
                var binary = Expect.Body as BinaryExpression;
                var access = binary.Left as MemberExpression;

                if (access != null)
                {
                    sb.Append(access.Member.Name);
                    sb.Append(" = ");
                    appendRight(binary.Right, sb);
                }
                else
                {

                    sb.Append(Expect.Body);
                }
            }
            else
            {
                sb.Append(Expect.Body);
            }

            return sb.ToString();
        }

        private static void appendRight(Expression exp, StringBuilder sb)
        {
            switch (exp.NodeType)
            {
                
                case ExpressionType.AddChecked:
                    break;
                case ExpressionType.And:
                    break;
                case ExpressionType.AndAlso:
                    break;
                case ExpressionType.ArrayLength:
                    break;
                case ExpressionType.ArrayIndex:
                    break;
                case ExpressionType.Call:
                    break;
                case ExpressionType.Coalesce:
                    break;
                case ExpressionType.Conditional:
                    break;
                case ExpressionType.Constant:
                    sb.Append(((ConstantExpression) exp).Value);
                    break;
                case ExpressionType.Convert:
                    break;
                case ExpressionType.ConvertChecked:
                    break;
                case ExpressionType.Equal:
                    break;
                case ExpressionType.ExclusiveOr:
                    break;
                case ExpressionType.GreaterThan:
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    break;
                case ExpressionType.Invoke:
                    break;
                case ExpressionType.Lambda:
                    break;
                case ExpressionType.LeftShift:
                    break;
                case ExpressionType.LessThan:
                    break;
                case ExpressionType.LessThanOrEqual:
                    break;
                case ExpressionType.ListInit:
                    break;
                case ExpressionType.MemberAccess:
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    object value = Expression.Lambda(exp).Compile().DynamicInvoke();
                    sb.Append(value);
                    break;
                case ExpressionType.MemberInit:
                    break;
                case ExpressionType.Modulo:
                    break;
                case ExpressionType.MultiplyChecked:
                    break;
                case ExpressionType.Negate:
                    break;
                case ExpressionType.UnaryPlus:
                    break;
                case ExpressionType.NegateChecked:
                    break;
                case ExpressionType.New:
                    break;
                case ExpressionType.NewArrayInit:
                    break;
                case ExpressionType.NewArrayBounds:
                    break;
                case ExpressionType.Not:
                    break;
                case ExpressionType.NotEqual:
                    break;
                case ExpressionType.Or:
                    break;
                case ExpressionType.OrElse:
                    break;
                case ExpressionType.Parameter:
                    break;
                case ExpressionType.Power:
                    break;
                case ExpressionType.Quote:
                    break;
                case ExpressionType.RightShift:
                    break;
                case ExpressionType.SubtractChecked:
                    break;
                case ExpressionType.TypeAs:
                    break;
                case ExpressionType.TypeIs:
                    break;
                case ExpressionType.Assign:
                    break;
                case ExpressionType.Block:
                    break;
                case ExpressionType.DebugInfo:
                    break;
                case ExpressionType.Decrement:
                    break;
                case ExpressionType.Dynamic:
                    break;
                case ExpressionType.Default:
                    break;
                case ExpressionType.Extension:
                    break;
                case ExpressionType.Goto:
                    break;
                case ExpressionType.Increment:
                    break;
                case ExpressionType.Index:
                    break;
                case ExpressionType.Label:
                    break;
                case ExpressionType.RuntimeVariables:
                    break;
                case ExpressionType.Loop:
                    break;
                case ExpressionType.Switch:
                    break;
                case ExpressionType.Throw:
                    break;
                case ExpressionType.Try:
                    break;
                case ExpressionType.Unbox:
                    break;
                case ExpressionType.AddAssign:
                    break;
                case ExpressionType.AndAssign:
                    break;
                case ExpressionType.DivideAssign:
                    break;
                case ExpressionType.ExclusiveOrAssign:
                    break;
                case ExpressionType.LeftShiftAssign:
                    break;
                case ExpressionType.ModuloAssign:
                    break;
                case ExpressionType.MultiplyAssign:
                    break;
                case ExpressionType.OrAssign:
                    break;
                case ExpressionType.PowerAssign:
                    break;
                case ExpressionType.RightShiftAssign:
                    break;
                case ExpressionType.SubtractAssign:
                    break;
                case ExpressionType.AddAssignChecked:
                    break;
                case ExpressionType.MultiplyAssignChecked:
                    break;
                case ExpressionType.SubtractAssignChecked:
                    break;
                case ExpressionType.PreIncrementAssign:
                    break;
                case ExpressionType.PreDecrementAssign:
                    break;
                case ExpressionType.PostIncrementAssign:
                    break;
                case ExpressionType.PostDecrementAssign:
                    break;
                case ExpressionType.TypeEqual:
                    break;
                case ExpressionType.OnesComplement:
                    break;
                case ExpressionType.IsTrue:
                    break;
                case ExpressionType.IsFalse:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}