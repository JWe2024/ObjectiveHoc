using Parser.Interfaces;
using System.Text;
using static Parser.Interfaces.ISymbolTable;

namespace Parser
{
    internal class LexicalAnalyser : ILexicalAnalyser
    {
        private TextReader input;
        private ISymbolTable symbolTable;
        private Queue<int> buffer;

        public LexicalAnalyser(TextReader input, ISymbolTable symbolTable)
        {
            this.input = input;
            this.symbolTable = symbolTable;
            this.buffer = new Queue<int>();
        }

        public ISymbolTable.IToken GetToken()
        {
            int c;

            while (true)
            {
                c = GetChar();

                if (c == -1)
                {
                    return symbolTable.GetToken(Convert.ToString(c));
                }
                else if (c == ' ' || c == '\t')
                {
                    //Skip white space
                }
                else if (c == '\n' || c == '\r')
                {
                    //Skip line feed and carriage return

                    c = GetChar();

                    return symbolTable.GetToken(Convert.ToString((char)c));
                }
                else if (Char.IsDigit((char)c) || c == '.')
                {
                    UngetChar(c);

                    decimal real = 0.0m;

                    ReadReal(ref real);

                    return symbolTable.GetToken(real);
                }
                else if (Char.IsLetter((char)c) || c == '_')
                {
                    UngetChar(c);

                    string id = string.Empty;

                    ReadIdentifier(ref id);

                    return symbolTable.GetToken(new Identifier(id));
                }
                else if(c == '\"')
                {
                    UngetChar(c);

                    string str = string.Empty;

                    ReadString(ref str);

                    return symbolTable.GetToken(new StringIdentifier(str));
                }
                else if (c == '<' || c == '>' || c == '=')
                {
                    return Follow(c, '=');
                }
                else if (c == '&')
                {
                    return Follow(c, '&');
                }
                else if (c == '|')
                {
                    return Follow(c, '|');
                }
                else if (c == '$')
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append((char)c);

                    while (true)
                    {
                        c = GetChar();

                        if (Char.IsDigit((char)c))
                        {
                            sb.Append((char)c);
                        }
                        else
                        {
                            UngetChar(c);
                            break;
                        }
                    }

                    return symbolTable.GetToken(new ArgIdentifier(sb.ToString()));
                }
                else
                {
                    return symbolTable.GetToken(Convert.ToString((char)c));
                }
            }
        }

        private ISymbolTable.IToken Follow(int c, int expected)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append((char)c);

            c = GetChar();

            if (c == expected)
            {
                sb.Append((char)c);

                string condition = sb.ToString();

                return symbolTable.GetToken(condition);
            }
            else
            {
                UngetChar(c);

                string condition = sb.ToString();

                return symbolTable.GetToken(condition);
            }
        }

        private int GetChar()
        {
            if (buffer.Count > 0)
            {
                return buffer.Dequeue();
            }
            else
            {
                return input.Read();
            }
        }

        private void UngetChar(int c)
        {
            buffer.Enqueue(c);
        }

        private void ReadIdentifier(ref string identifier)
        {
            StringBuilder sb = new StringBuilder();

            int c = GetChar();

            if (Char.IsLetter((char)c) || c == '_')
            {
                sb.Append((char)c);

                c = GetChar();

                while (Char.IsLetter((char)c) || Char.IsDigit((char)c) || c == '_')
                {
                    sb.Append((char)c);

                    c = GetChar();
                }

                UngetChar(c);

                identifier = sb.ToString();
            }
            else
            {
                UngetChar(c);
            }
        }

        private void ReadString(ref string str)
        {
            StringBuilder sb = new StringBuilder();

            int c = GetChar();

            if (c == '\"')
            {
                while (true)
                {
                    c = GetChar();

                    if (c == -1 || c == '\n' || c == '\t')
                    {
                        throw new Exception("syntax error: missing closing quotation mark");
                    }
                    else if (c == '\"')
                    {
                        c = GetChar();

                        break;
                    }
                    else
                    {
                        sb.Append((char)c);
                    }
                }

                UngetChar(c);

                str = sb.ToString().Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\t", "\t").Replace("\\b", "\b").Replace("\\f", "\f");
            }
            else
            {
                UngetChar(c);
            }
        }

        private void ReadReal(ref decimal real)
        {
            StringBuilder sb = new StringBuilder();

            // Try to parse <significand> followed by <exponent_part>
            if (ParseSignificand(sb) && ParseExponentPart(sb))
            {
                real = decimal.Parse(sb.ToString(), System.Globalization.NumberStyles.Float);
            }
            else
            {
                throw new Exception("syntax error: invalid input for decimal number");
            }
        }

        private bool ParseSignificand(StringBuilder sb)
        {
            // Try to parse <integer_part>
            if (ParseIntegerPart(sb))
            {
                // Check for optional decimal point and <fractional_part>
                int c = GetChar();

                if (c == '.')
                {
                    sb.Append((char)c);

                    return ParseFractionalPart(sb);
                }

                // No decimal point and fractional part needed
                UngetChar(c);

                return true;
            }
            else
            {
                int c = GetChar();

                if (c == '.')
                {
                    sb.Append((char)c);

                    return ParseFractionalPart(sb);
                }
            }

            return false;
        }

        private bool ParseIntegerPart(StringBuilder sb)
        {
            // Try to parse at least one <digit>
            if (ParseDigit(sb))
            {
                // Continue parsing <digit> <integer_part>
                int c = GetChar();

                while (Char.IsDigit((char)c))
                {
                    sb.Append((char)c);

                    c = GetChar();
                }

                UngetChar(c);

                return true;
            }

            return false;
        }

        private bool ParseDigit(StringBuilder sb)
        {
            int c = GetChar();

            if (Char.IsDigit((char)c))
            {
                sb.Append((char)c);

                return true;
            }

            UngetChar(c);

            return false;
        }

        private bool ParseFractionalPart(StringBuilder sb)
        {
            // Try to parse at least one <digit>
            if (ParseDigit(sb))
            {
                // Continue parsing <digit> <fractional_part>
                int c = GetChar();

                while (Char.IsDigit((char)c))
                {
                    sb.Append((char)c);

                    c = GetChar();
                }

                UngetChar(c);

                return true;
            }

            return false;
        }

        private bool ParseExponentPart(StringBuilder sb)
        {
            // Check for 'e' or 'E' indicating start of <exponent_part>
            int c = GetChar();

            if (c == 'e' || c == 'E')
            {
                sb.Append((char)c);

                return ParseExponentSign(sb) && ParseExponentValue(sb);
            }

            // No exponent part needed (optional)
            UngetChar(c);

            return true;
        }

        private bool ParseExponentSign(StringBuilder sb)
        {
            // Check for optional '+' or '-' sign
            int c = GetChar();

            if (c == '+' || c == '-')
            {
                sb.Append((char)c);
            }

            UngetChar(c);

            // Sign is optional
            return true;
        }

        private bool ParseExponentValue(StringBuilder sb)
        {
            // Try to parse at least one <digit>
            if (ParseDigit(sb))
            {
                // Continue parsing <digit> <exponent_value>
                int c = GetChar();
                while (Char.IsDigit((char)c))
                {
                    sb.Append((char)c);
                }

                return true;
            }

            return false;
        }
    }
}
