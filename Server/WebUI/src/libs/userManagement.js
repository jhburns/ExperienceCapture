import { gapi } from 'gapi-script';

function submitUser(isMock=false, onError) {
	if (isMock) {
		
		return;
	}
}

//function postUser(data, onError {
//
//}

function signOutUser(isMock=false) {
  if (isMock) {
	return;
  }

  const auth2 = gapi.auth2.getAuthInstance();
  auth2.signOut().then(() => {});
}

export { submitUser, signOutUser };