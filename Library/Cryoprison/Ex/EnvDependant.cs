namespace Cryoprison.Ex
{
    public class EnvDependant
    {
        protected Env Env { get; set; }

        protected EnvDependant(Env env = null)
        {
            this.Env = env;
        }
    }
}
