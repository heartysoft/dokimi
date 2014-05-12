using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace dokimi.core
{
    public class ExceptionExpectation<T> : Expectation where T:Exception
    {
        private readonly Expression<Func<T, bool>> _expression;
        private readonly string _description;

        public ExceptionExpectation(string description = null)
            : this(null, description)
        {
        }

        public ExceptionExpectation(Expression<Func<T, bool>> expression, string description = null)
        {
            _expression = expression;
            _description = description;
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
            if (input.Length != 1)
                throw new ExpectationFailedException(this, input);

            var exception = input[0] as T;

            if (exception == null)
                throw new ExpectationFailedException(this, input);

            if (_expression == null) //expression is null, but exception is of type T. So, pass.
                return;

            if (_expression.Compile()(exception) == false)
                throw new ExpectationFailedException(this, input);
        }

        public override string ToString()
        {
            if (string.IsNullOrWhiteSpace(_description) == false)
                return _description;
            
            if (_expression == null)
                return getPrettyPrintPrefix();

            return string.Format("{0} with {1}", getPrettyPrintPrefix(), getPrettyPrintSuffix());
        }

        private static string getPrettyPrintPrefix()
        {
            return string.Format("[Error] {0}", typeof(T).Name.Replace("Exception", string.Empty).ToWords());
        }

        private string getPrettyPrintSuffix()
        {
            var sb = new StringBuilder();
            if (_expression.Body.NodeType == ExpressionType.Equal)
            {
                var binary = _expression.Body as BinaryExpression;
                var access = binary.Left as MemberExpression;

                if (access != null)
                {
                    sb.Append(access.Member.Name);
                    sb.Append(" = ");
                    appendRight(binary.Right, sb);
                }
                else
                {
                    sb.Append(_expression.Body);
                }
            }
            else
            {
                sb.Append(_expression.Body);
            }

            return sb.ToString();
        }

        //TODO: refactor later..same thing in Expect<T>.
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
                    sb.Append(((ConstantExpression)exp).Value);
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