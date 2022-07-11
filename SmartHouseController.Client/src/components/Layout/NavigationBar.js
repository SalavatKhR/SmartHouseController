import React from 'react';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link } from 'react-router-dom'

const NavigationBar = () => {
    const handleLogout = async () => {
        document.cookie = 'smart_token=; Path=/; Expires=Thu, 01 Jan 1970 00:00:01 GMT;';
    }
    return (
        <div>
            <Navbar bg="primary" variant="dark">
                <Container>
                    <Navbar.Brand style={{fontWeight: 'bold'}} as={Link} to="/">Smart House Controller</Navbar.Brand>
                    <Nav className="me-auto">
                        <Nav.Link as={Link} to="/">Dashboard</Nav.Link>
                        <Nav.Link as={Link} onClick={handleLogout} to="/login">Logout</Nav.Link>
                    </Nav>
                </Container>
            </Navbar>
        </div>
    );
};

export default NavigationBar;