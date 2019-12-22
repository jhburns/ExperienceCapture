import { gapi } from 'gapi-script';

import { postData } from 'libs/fetchExtra';

async function submitUser(isMock=false, onError) {
	const profile = gapi.currentUser.get();
	console.log("ID: " + profile.getId());
	console.log('Full Name: ' + profile.getName());
	console.log('Given Name: ' + profile.getGivenName());
	console.log('Family Name: ' + profile.getFamilyName());
	console.log("Email: " + profile.getEmail());
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