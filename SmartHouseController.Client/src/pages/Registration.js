import React from 'react';
import Form from "react-bootstrap/Form";
import Button from "react-bootstrap/Button";
import {Link} from "react-router-dom";
import axios from "axios";

const Registration = () => {
    const [email, setEmail] = React.useState("");
    const [password, setPassword] = React.useState("");
    const [repeatPassword, setRepeatPassword] = React.useState("");
    const [validated, setValidated] = React.useState(false);
    const [error, setError] = React.useState("");

    const handleSubmit = (e) => {
        e.preventDefault();
        const formCheck = e.currentTarget;
        if (formCheck.checkValidity() === false) {
            e.stopPropagation();
        }

        setValidated(true);

        let form = new URLSearchParams();
        form.append('grant_type', 'password');
        form.append('username', email);
        form.append('password', password);

        axios.post("https://localhost:7085/api/auth/signup", form)
            .then((res) => {
                window.location.replace("/");
                document.cookie = 'smart_token='+ res.data['access_token']
                + ';'+ 'path=/;' + 'expires=' + res.data['expires_in'];
            })
            .catch(res => {
                if (res.response.status === 400)
                    setError("User with this email is already registered.");
            })
    }

    return (
        <div>
            <Form
                noValidate validated={validated}
                style={{
                    position: "absolute",
                    left: "35%",
                    top: "30%",
                    width: "30%"
                }}
                onChange={() => setError("")}
                onSubmit={handleSubmit}
            >
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
                <Form.Group className="mb-3" controlId="formBasicRepeatPassword">
                    <Form.Label>Password</Form.Label>
                    <Form.Control
                        value={repeatPassword}
                        onChange={e => setRepeatPassword(e.target.value)}
                        type="password"
                        placeholder="Repeat password"
                        required
                    />
                    <Form.Control.Feedback type="invalid">
                        Passwords must match.
                    </Form.Control.Feedback>
                </Form.Group>
                <Button variant="primary" type="submit">
                    Register
                </Button>
                <p style={{textAlign: 'center', marginTop: '5px'}}>Already have an account? <Link
                    to='/login'>Login</Link></p>
                {
                    error ? <p style={{color: 'red', textAlign: 'center'}}>{error}</p> : ""
                }
            </Form>
        </div>
    );
};

export default Registration;