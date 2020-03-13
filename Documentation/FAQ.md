# Frequently Asked Questions

### How can I change the client?

[comment]: <> (TODO: Document CaptureSetup)

See the properties on the CaptureSetup prefab in the Unity Editor. There isn't documentation
on it yet, but hovering over properties has tool-tips that should help explain. 

### What happens if the game crashes/etc?

The reality is there are a number of ways as session could break, including but not limited to:
- The game crashes.
- The game client crashes.
- The network between the game and the server cuts out.
- The server crashes or becomes overwhelmed and times out.
- AWS, the hosting platform, having an outage that effects the server.

The main strategy Experience Capture uses to deal with all of there is to transmit the data to the back-end as soon as it is produced. So in the worst case less than a second of data is ever lost.

## How can I delete sessions?

It is impossible to delete a session, or any data for that matter. However, sessions can be archived to hide them, which is analogous to a delete.

## How to find all capturable objects?

`Ctrl-Shift-F` and then search for `ICapturable` across the whole project in Visual Studios.

## How can I sign up/sign in on a mobile device?

When signing up be careful because mobile Chrome will automatically sign you into whatever account is currently synced. If you want to use a specific account either disable syncing temporarily (Settings > Account > Sign out and turn off sync) or add an account. The same goes for signing in, as you can only sign in using the account that was used to sign up. 