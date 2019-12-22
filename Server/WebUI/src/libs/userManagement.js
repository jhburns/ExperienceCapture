import { gapi } from 'gapi-script';

function submitUser(isMock=false, onError) {
	if (isMock) {
		return;
	}
}

//function postUser(data, onError {
//
//}

async function signOutUser(isMock=false) {
  if (isMock) {
	return;
  }

  const auth2 = gapi.auth2.getAuthInstance();
  await auth2.signOut();
}

export { submitUser, signOutUser };