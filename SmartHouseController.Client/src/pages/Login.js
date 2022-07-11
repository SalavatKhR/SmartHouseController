import React, {useState} from 'react';
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import {Link} from "react-router-dom";
import axios from "axios";

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const handleSubmit = (e) => {
        e.preventDefault();

        let form = new URLSearchParams();
        form.append('grant_type', 'password');
        form.append('username', email);
        form.append('password', password);

        axios.post("https://localhost:7085/api/auth/login", form)
            .then((res) => {
                window.location.replace("/");
                document.cookie = 'smart_token='+ res.data['access_token']
                    + ';'+ 'path=/;' + 'expires=' + res.data['expires_in'];
            })
            .catch(res => {
                if (res.response.status === 400)
                    setError("Couldn't find user with such credentials.");
            })
    }

    return (
            <div>
                <Form style={{
                    position: "absolute",
                    left: "35%",
                    top: "30%",
                    width: "30%"
                }}
                onSubmit={handleSubmit}>

                    <Form.Group className="mb-3" controlId="formBasicEmail">
                        <Form.Label>Email address</Form.Label>
                        <Form.Control
                            value={email}
                            onChange={e => setEmail(e.target.value)}
                            type="email"
                            placeholder="Enter email"
                        />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="formBasicPassword">
                        <Form.Label>Password</Form.Label>
                        <Form.Control
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            type="password"
                            placeholder="Password"
                        />
                    </Form.Group>
                    <Button variant="primary" type="submit">
                        Submit
                    </Button>
                    <p style={{textAlign: 'center', marginTop: '5px'}}>Don't have an account? <Link
                        to='/registration'>Register</Link></p>
                    {
                        error ? <p style={{color: 'red', textAlign: 'center'}}>{error}</p> : ""
                    }
                </Form>
            </div>
    );
};

export default Login;