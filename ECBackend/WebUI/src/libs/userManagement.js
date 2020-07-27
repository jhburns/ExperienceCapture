/* global gapi */

import { postData } from 'libs/fetchExtra';
import { createCookie } from 'libs/cookieExtra';

import { environmentVariables } from "libs/environment";

// Has to be the same as the backend shim
// Long to match the length of real subjects
const mockId = "123456789109876543210";

const mockToken = "This.is.not.a.real.id.token";

// Load the website from root in order to set it
// As running in mock mode.
let isMockFromRoot = false;

/**
 * Decides whether to sign up a user, fulfil a claim, or sign in a user.
 *
 * @param {boolean} isMock - When true the site is mocking Google Sign-in.
 * @param {object} user - A user to submit.
 * @param {Function} onError - Callback if an error occurs.
 * @param {object} options - Choose which way to submit the user, default is a normal sign in.
 * @param {Function} onDuplicate - Optional, and only needed when signing a user up.
 */
async function submitUser(
  isMock = false,
  user,
  onError,
  options = { signUpToken: undefined, claimToken: undefined },
  onDuplicate) {
  isMockFromRoot = isMock;

  if (options.signUpToken !== undefined) {
    await signUpUser(user, options.signUpToken, onError, onDuplicate);
    return;
  }

  if (options.claimToken !== undefined) {
    await fulfillClaim(user, options.claimToken, onError);
    return;
  }

  await signInUser(user, onError);
}

/**
 * Sign up a user.
 *
 * @param {object} user - A user to sign up.
 * @param {string} signUpToken - Base64 token that is passed to the back-end to sign up.
 * @param {Function} onError - Callback if an error occurs.
 * @param {Function} onDuplicate - Callback if the user already exists.
 */
async function signUpUser(user, signUpToken, onError, onDuplicate) {
  let userData = {
    idToken: mockToken,
    signUpToken: signUpToken,
  };

  if (!isMockFromRoot) {
    userData = {
      idToken: user.getAuthResponse().id_token,
      signUpToken: signUpToken,
    };
  }

  try {
    const replyData = await postData('/api/v1/users/', userData);

    if (!replyData.ok) {
      if (replyData.status === 409) {
        onDuplicate();
      } else {
        throw Error(replyData.status);
      }
    }

    await signInUser(user, onError);
  } catch (error) {
    console.error(error);
    onError();
  }
}

/**
 * Fulfills a claim.
 *
 * @param {object} user - A user to fulfill a claim for.
 * @param {string} claimToken - Base64 token that is passed to the back-end.
 * @param {Function} onError - Callback if an error occurs.
 */
async function fulfillClaim(user, claimToken, onError) {
  let userData = {
    idToken: mockToken,
    claimToken: claimToken,
  };

  let userId = mockId;

  if (!isMockFromRoot) {
    userData = {
      idToken: user.getAuthResponse().id_token,
      claimToken: claimToken,
    };

    userId = user.getId();
  }

  try {
    const replyData = await postData(`/api/v1/users/${userId}/tokens/`, userData);

    if (!replyData.ok) {
      throw Error(replyData.status);
    }
  } catch (error) {
    console.error(error);
    onError();
  }
}

/**
 * Sign in a user.
 *
 * @param {object} user - A user to submit.
 * @param {Function} onError - Callback if an error occurs.
 */
async function signInUser(user, onError) {
  let userData = {
    idToken: mockToken,
  };

  let userId = mockId;

  if (!isMockFromRoot) {
    userData = {
      idToken: user.getAuthResponse().id_token,
    };

    userId = user.getId();
  }

  try {
    const replyData = await postData(`/api/v1/users/${userId}/tokens/`, userData);

    if (!replyData.ok) {
      throw Error(replyData.status);
    }

    const response = await replyData.json();
    createCookie("ExperienceCapture-Access-Token", response.accessToken);
  } catch (error) {
    console.error(error);
    onError();
  }
}

/**
 * Sign out a user.
 *
 * @param {Function} onSuccess - Callback when the user is signed out.
 * @param {Function} onError - Callback when there is an issue with authorization.
 */
async function signOutUser(onSuccess, onError) {
  if (isMockFromRoot) {
    onSuccess();
    return;
  }

  gapi.auth2.init({
    client_id: environmentVariables["REACT_APP_GOOGLE_CLIENT_ID"],
  }).then(async () => {
    try {
      const auth2 = gapi.auth2.getAuthInstance();
      await auth2.signOut();
    } catch (err) {
      onError(err);
    }
  },
  () => {
    onError(new Error("User is not signed-in when loading the settings page."));
  });
}

/**
 * Get the signed in user's id.
 *
 * @param {Function} onId - Callback when an id can successfully be found.
 * @param {Function} onError - Callback when there is an issue with authorization.
 */
function getUserId(onId, onError) {
  if (isMockFromRoot) {
    onId(mockId);
    return;
  }

  gapi.auth2.init({
    client_id: environmentVariables["REACT_APP_GOOGLE_CLIENT_ID"],
  }).then(() => {
    try {
      const auth2 = gapi.auth2.getAuthInstance();

      var profile = auth2.currentUser.get().getBasicProfile();
      onId(profile.getId());
    } catch (err) {
      onError(err);
    }
  },
  () => {
    onError(new Error("User is not signed-in when loading the settings page."));
  });
}

export { submitUser, signOutUser, getUserId };