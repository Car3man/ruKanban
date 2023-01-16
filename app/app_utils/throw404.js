const throw404 = (req, res) => {
    res.status(404).send("API doesn't exist")
}

module.exports = throw404