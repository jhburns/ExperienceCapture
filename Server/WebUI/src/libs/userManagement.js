import { gapi } from 'gapi-script';

import { postData } from 'libs/fetchExtra';

async function submitUser(isMock=false, user, onError) {
	var userData = {
		idToken: "This.is.not.a.real.id.token",
		id: 4321234,
		signUpToken: "waa"
	};

	if (!isMock) {
		userData = {
			idToken: user.getAuthResponse().id_token,
			id: user.getId(),
			signUpToken: "waa"
		};
	}

	try {
		const replyData = await postData('/api/v1/users/', userData);
		console.log(JSON.stringify(replyData));
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