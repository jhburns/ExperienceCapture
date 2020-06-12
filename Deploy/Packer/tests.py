def test_passwd_file(host):
    passwd = host.file("/etc/passwd")
    assert passwd.contains("root")
    assert passwd.user == "root"
    assert passwd.group == "root"
    assert passwd.mode == 0o644

def test__ec_debug_user(host):
    ec_debug = host.user("ec-debug")
    assert "docker" in ec_debug.group

def test_docker_is_installed(host):
    docker = host.package("docker-ce")
    assert docker.is_installed
    assert docker.version.startswith("5:19.03")

def test_google_is_reachable(host):
    google = host.addr("google.com")
    assert google.is_resolvable

def test_docker_is_listening(host):
    docker = host.socket("unix:///var/run/docker.sock")
    assert docker.is_listening
