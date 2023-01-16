const signUp = (req, res) => {
    res.status(200).send("signUp")
};

const signIn = (req, res) => {
    res.status(200).send("signIn")
};

const signOut = (req, res) => {
    res.status(200).send("signOut")
};

const changePassword = (req, res) => {
    res.status(200).send("changePassword")
};

module.exports = { signUp, signIn, signOut, changePassword }