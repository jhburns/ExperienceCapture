/**
 * Force the application to start with certain environment variables.
 *
 * @param {Array<string>} expectedVars - Environmental variables to check.
 */
function verifyEnvironment(expectedVars)
{
  for (const envVar of expectedVars) {
    if (!(envVar in process.env)) {
      throw new Error(`The following environment variable is unset: ${envVar}`);
    }
  }
}

export { verifyEnvironment };