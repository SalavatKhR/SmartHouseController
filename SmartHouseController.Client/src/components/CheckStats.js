import React from 'react';
import Modal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

const CheckStats = ({show, setShow, device, deviceStats}) => {
    const handleClose = () => setShow(false);
    const handleShow = () => setShow(true);
    const key = Object.keys(deviceStats)[0];

    return (
        <>
            <Modal show={show} onHide={handleClose}>
                <Modal.Header closeButton>
                    <Modal.Title>{device ? device['deviceName'] : ""}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div>
                    {
                        deviceStats[key]
                            ? Object.keys(deviceStats[key]).map((keySec, index) => {
                                if (keySec === "payload") {
                                    let payload = JSON.parse(deviceStats[key][keySec]);
                                    return Object.keys(payload)
                                        .map((keyThird, indexThird) =>
                                            <p key={keyThird+keySec}>{keyThird}: {payload[keyThird]}</p>)
                                } else
                                    return (<p key={keySec}>{keySec}: {deviceStats[key][keySec]}</p>)
                            }) : "Soon controller will send notification!"
                    }
                    </div>
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={handleClose}>
                        Close
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
};

export default CheckStats;