import axios from 'axios';
import { Link } from 'react-router-dom';


const LogoutComponent = () => {
    const handleLogout = async () => {
        try {
            await axios.post('http://localhost:5001/api/authentication/Logout');

            axios.defaults.headers.common['Authorization'] = null;
            localStorage.removeItem('jwtToken');

        } catch (error) {
        }
    };

    return (
        <div>
            <h1>Logout from system</h1>
            <button onClick={handleLogout}>Logout</button>
            <Link to="/">Go to Main Page</Link>
        </div>
    );
};

export default LogoutComponent;