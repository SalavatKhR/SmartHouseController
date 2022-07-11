import React, {useEffect, useState} from 'react';
import Layout from "../components/Layout/Layout";
import Card from 'react-bootstrap/Card'
import Button from 'react-bootstrap/Button'
import AddDevice from "./AddDevice";
import getCookie from "../components/getCookie";
import axios from "axios";
import {HttpTransportType, HubConnectionBuilder} from "@microsoft/signalr";
import CheckStats from "../components/CheckStats";

const Dashboard = () => {
    const [devices, setDevices] = useState([]);
    const [passedDevice, setPassedDevice] = useState(null)
    const [stats, setStats] = useState([]);
    const [connection, setConnection] = useState(null);
    const [show, setShow] = useState(false);
    const addDevice = (newDevice) => {
        setDevices([...devices, newDevice])
    }
    const handleRemove = (topic) => {
        axios.delete("https://localhost:7085/api/subscriptions?topic=" + topic, {
            headers: {
                'Authorization': 'Bearer ' + getCookie()
            }
        })
            .then(() => setDevices((devices) =>
                devices.filter((device) => device.topic !== topic)))
            .catch(console.log);
    }
    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl('https://localhost:7085/hub', {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect()
            .build();
        setConnection(newConnection);
        newConnection.onclose(() => connection.invoke("OnDisconnectedAsync", getCookie()));
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(result => {
                    console.log('Connected!');
                    connection.invoke("GetUpdates", getCookie());
                    connection.on('GetUpdates', message => {
                        console.log(message)
                        stats['topic'] = message;
                        setStats(stats);
                    });
                })
                .catch(e => console.log('Connection failed: ', e));
        }
    }, [connection]);

    useEffect(() => {
        axios.get("https://localhost:7085/api/subscriptions", {
            headers: {
                'Authorization': 'Bearer ' + getCookie()
            }
        })
            .then(res => setDevices(res.data))
            .catch(console.log);
    }, [])

    let index = 0;
    return (
        <Layout>
            <AddDevice style={{
                marginLeft: '45%',
                marginTop: '20px',
                fontSize: '30px',
                padding: '5px',
                paddingRight: '100px',
                paddingLeft: '100px',
            }} add={addDevice}/>
            <CheckStats
                show={show}
                setShow={setShow}
                device={passedDevice}
                deviceStats={stats}
            />
            <div style={{display: 'flex', flexWrap: 'wrap'}}>
                {
                    devices.map(device => {
                        index++
                        return (
                            <div key={device.topic}>
                                <Card style={{width: '30rem', marginLeft: '5rem', marginTop: '2rem'}}>
                                    <Card.Body>
                                        <Card.Title>{index}. {device['deviceName']} ({device['topic']})</Card.Title>
                                        <Card.Text>
                                            {device['deviceDescription']}
                                        </Card.Text>
                                        <Button
                                            variant="primary"
                                            onClick={() => {
                                                setPassedDevice(device);
                                                setShow(true);}}
                                        >Check stats
                                        </Button>
                                        <Button onClick={() => handleRemove(device.topic)} style={{marginLeft: '1rem'}}
                                                variant="danger">Remove</Button>
                                    </Card.Body>
                                </Card>
                            </div>
                        )
                    })
                }
            </div>
        </Layout>
    );
};

export default Dashboard;