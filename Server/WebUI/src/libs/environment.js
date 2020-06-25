
function verifyEnvironment(expectedVars)
{
  for (const envVar of expectedVars) {
    if (!(envVar in process.env)) {
      throw new Error(`The following environment variable is unset: ${envVar}`);
    }
  }
}

export { verifyEnvironment };