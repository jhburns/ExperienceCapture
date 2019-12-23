import { gapi } from 'gapi-script';

import { postData } from 'libs/fetchExtra';

async function submitUser(isMock=false, user, onError, onDuplicate) {
	var userData = {
		idToken: "This.is.not.a.real.id.token",
		id: 4321234,
		signUpToken: "ctPHOKJkLbCom5JrT4E7BupeyKVqQVb6Kgs+ZfHW3mI="
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

		if (!replyData.ok) {
			if (replyData.status === 409) {
				onDuplicate();
			} else {
				throw Error(replyData.status);
			}
		}

		const token = replyData.text();
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