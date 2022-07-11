import React, {useState} from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from "react-bootstrap/Button";
import Form from 'react-bootstrap/Form';
import axios from "axios";
import getCookie from "../components/getCookie";

const AddDevice = ({add, style}) => {
    const [show, setShow] = useState(false);
    const [device, setDevice] = useState({deviceName: '', deviceDescription: '', topic: ''})
    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);
    const [validated, setValidated] = React.useState(false);
    const [error, setError] = React.useState("");

    const handleSubmit = (e) => {
        e.preventDefault();
        const formCheck = e.currentTarget;
        if (formCheck.checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            return;
        }

        let form = new URLSearchParams();
        form.append('DeviceName', device.deviceName);
        form.append('DeviceDescription', device.deviceDescription);
        form.append('Topic', device.topic);

        axios.post("https://localhost:7085/api/Subscriptions", form,
            {
                headers: {
                    'Authorization': 'Bearer ' + getCookie()
                }
             })
            .then((res) => {
                add(device);
                setValidated(true);
                handleClose();
                setDevice({deviceName: '', deviceDescription: '', topic: ''})
            })
            .catch(console.log);
    }

    return (
        <>
            <Button style={style} variant="success" onClick={handleShow}>+</Button>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>Add device</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form
                        id="form_add"
                        validated={validated}
                        onChange={() => setError("")}
                        onSubmit={handleSubmit}
                    >
                        <Form.Group className="mb-3">
                            <Form.Label>Name</Form.Label>
                            <Form.Control
                                value={device.deviceName}
                                onChange={e => setDevice({...device, deviceName: e.target.value})}
                                type="text"
                                required
                            />
                            <Form.Control.Feedback type="invalid">
                                Field cannot be empty.
                            </Form.Control.Feedback>
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Description</Form.Label>
                            <Form.Control
                                value={device.deviceDescription}
                                onChange={e => setDevice({...device, deviceDescription: e.target.value})}
                                type="text"
                            />
                        </Form.Group>
                        <Form.Group className="mb-3">
                            <Form.Label>Topic</Form.Label>
                            <Form.Control
                                value={device.topic}
                                onChange={e => setDevice({...device, topic: e.target.value})}
                                type="text"
                                required
                            />
                            <Form.Control.Feedback type="invalid">
                                Field cannot be empty.
                            </Form.Control.Feedback>
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                    <Button variant="primary" form="form_add" type="submit">
                        Add
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default AddDevice;