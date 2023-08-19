import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const CreateUser = () => {
    const [nickName, setNickName] = useState('');
    const [email, setEmail] = useState('');
    const [comments, setComments] = useState('');
    const [status, setStatus] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            const token = localStorage.getItem('jwtToken');
            const response = await axios.post('http://localhost:5001/api/UserService/CreateUser', {
                NickName: nickName,
                Email: email,
                Comments: comments
            }, {
                headers: {
                    'Authorization': `Bearer ${token}`
                }
            });

            if (response.status === 201) {
                setStatus(`User created successfully. Email: ${response.data.email}`);
            } else {
                setStatus('Error creating user.');
            }
        } catch (error) {
            setStatus('Error creating user.');
        }
    };

    return (
        <div>
            <h2>Create User</h2>
            <form onSubmit={handleSubmit}>
                <div>
                    <label>NickName:</label>
                    <input type="text" value={nickName} onChange={(e) => setNickName(e.target.value)} required />
                </div>
                <div>
                    <label>Email:</label>
                    <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
                </div>
                <div>
                    <label>Comments:</label>
                    <input type="text" value={comments} onChange={(e) => setComments(e.target.value)} />
                </div>
                <button type="submit">Create</button>
            </form>
            <p>Status: {status}</p>
            <Link to="/">Go to Main Page</Link>
        </div>
    );
}

export default CreateUser;