import { gapi } from 'gapi-script';

import { postData } from 'libs/fetchExtra';

async function submitUser(isMock=false, user, onError, options={ signUpToken: undefined, claimToken: undefined }, onDuplicate) {
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

async function signUpUser(isMock=true, user, signUpToken, onError, onDuplicate) {
	console.log("pls");
	let userData = {
		idToken: "This.is.not.a.real.id.token",
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
	} catch (error) {
		console.error(error);
		onError();
	}
}

async function fulfillClaim(isMock=true, user, claimToken, onError) {
	// Fill in
}

async function signInUser(isMock=true, user, onError) {
	let userData = {
		idToken: "This.is.not.a.real.id.token",
	};

	let userId = "123456789109876543210"; // Has to be the same as the backend shim

	if (!isMock) {
		userData = {
			idToken: user.getAuthResponse().id_token,
		};

		userId = user.getId();
		console.log(userId);
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

async function signOutUser(isMock=false) {
  if (isMock) {
	return;
  }

  const auth2 = gapi.auth2.getAuthInstance();
  await auth2.signOut();
}

export { submitUser, signOutUser };