/* global gapi */

import { postData } from 'libs/fetchExtra';
import { createCookie } from 'libs/cookieExtra';

// Has to be the same as the backend shim
// Long to match the length of real subjects
const mockId = "123456789109876543210";

const mockToken = "This.is.not.a.real.id.token";

async function submitUser(
	isMock = false,
	user,
	onError,
	options = { signUpToken: undefined, claimToken: undefined },
	onDuplicate) {
	if (options.signUpToken !== undefined) {
		await signUpUser(isMock, user, options.signUpToken, onError, onDuplicate);
		return;
	}

	if (options.claimToken !== undefined) {
		await fulfillClaim(isMock, user, options.claimToken, onError);
		return;
	}

	await signInUser(isMock, user, onError);
}

async function signUpUser(isMock = true, user, signUpToken, onError, onDuplicate) {
	let userData = {
		idToken: mockToken,
		signUpToken: signUpToken
	};

	if (!isMock) {
		userData = {
			idToken: user.getAuthResponse().id_token,
			signUpToken: signUpToken
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

		await signInUser(isMock, user, onError);
	} catch (error) {
		console.error(error);
		onError();
	}
}

async function fulfillClaim(isMock = true, user, claimToken, onError) {
	let userData = {
		idToken: mockToken,
		claimToken: claimToken
	};

	let userId = mockId;

	if (!isMock) {
		userData = {
			idToken: user.getAuthResponse().id_token,
			claimToken: claimToken
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
	}}

async function signInUser(isMock = true, user, onError) {
	let userData = {
		idToken: mockToken,
	};

	let userId = mockId;

	if (!isMock) {
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

async function signOutUser(isMock = false) {
  if (isMock) {
		return;
  }

	try {
  	const auth2 = gapi.auth2.getAuthInstance();
		await auth2.signOut();
	} catch (err) {
		console.error(err);

		if (!isMock) {
			throw err;
		}
	}
}

function getUserId() {
	try {
		const auth2 = gapi.auth2.getAuthInstance();

		if (auth2.isSignedIn.get()) {
			var profile = auth2.currentUser.get().getBasicProfile();
			return profile.getId();
		} else {
			throw new Error("User is not signed-in when loading the settings page.");
		}
	} catch (err) {
		console.log("Application is running using mock data.");
		console.log(err);

		return mockId;
	}
}

export { submitUser, signOutUser, getUserId, };