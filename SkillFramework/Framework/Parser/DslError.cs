using System;

namespace SkillFramework
{
#if FULL_VERSION
    class DslError
    {
        internal DslError(DslToken tokens)
        {
            this.tokens = tokens;
        }

        internal bool HasError
        {
            get { return mHasError; }
        }

        internal short mismatch(short terminal, short token)
        {
            mHasError = true;
            Logger.Error(" expecting {0} but found {1}", DslString.GetSymbolName(terminal), DslString.GetSymbolName(token));
            return token;
        }

        internal short no_entry(short nonterminal, short token, int level)
        {
            mHasError = true;
            Logger.Error(" syntax error: skipping input {0}", DslString.GetSymbolName(token));
            token = tokens.get(); // advance the input
            return token;
        }

        internal void input_left()
        {
            Logger.Error("input left.");
        }

        internal void message(string message)
        {
            Logger.Error("{0}", message);
        }

        private DslToken tokens;
        private bool mHasError;
    }
#endif
}
