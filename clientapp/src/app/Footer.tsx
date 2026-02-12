'use client'

import { Container, Row, Col } from 'react-bootstrap';
import styles from './Footer.module.css';
import Link from 'next/link';

export default function Footer() {
    return (
        <footer className={styles.customFooter}>
            <Container className={styles.footerContainer}>
                <Row className={styles.footerRow}>
                    <Col md={6} className={styles.footerColumn}>
                        <h3 className={styles.footerHeader}>Email</h3>
                        <p className={styles.footerText}>gloriagronowicz@gmail.com</p>
                    </Col>
                    <Col md={6} className={styles.footerColumn}>
                        <h3 className={styles.footerHeader}>Phone</h3>
                        <p className={styles.footerText}>860.670.0799</p>
                    </Col>
                </Row>
                <Row className={styles.subscribeRow}>
                    <Col className="text-center">
                        <Link href="/subscribe" className={styles.subscribeLink}>
                            Subscribe for email updates
                        </Link>
                    </Col>
                </Row>
            </Container>
        </footer>
    );
}