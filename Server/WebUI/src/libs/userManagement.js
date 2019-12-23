import { gapi } from 'gapi-script';

import { postData } from 'libs/fetchExtra';

async function submitUser(isMock=false, user, onError) {
	console.log("ID: " + user.getId());
	console.log("secure token: " + user.getAuthResponse().id_token);
	try {
		const replyData = await postData('/api/v1/', {});
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